using System;
using System.Linq;
using INUlib.Core.Math;
using System.Collections.Generic;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Struct containing all cooldown info for the ability.
    /// Current Cooldown, Total Cooldown and Total Coodlwon without the Cooldown reduction
    /// </summary>
    public class CooldownInfo
    {
        /// <summary>
        /// The ability current cooldown
        /// </summary>
        public readonly float currentCooldown;
        
        /// <summary>
        /// The maximum cooldown of the ability, considering cooldown reductions
        /// </summary>
        public readonly float maxCooldown;
        
        /// <summary>
        /// The raw maximum cooldown of the ability, not counting cooldown reductions
        /// </summary>
        public readonly float maxCooldownWithoutCdr;
        
        /// <summary>
        /// How many charges are available to the ability
        /// </summary>
        public readonly int availableCharges;
        
        /// <summary>
        /// The max amount of charges the ability can have
        /// </summary>
        public readonly int maxCharges;
        
        /// <summary>
        /// The ability current amount of temporary charges
        /// </summary>
        public readonly int extraCharges;
        
        /// <summary>
        /// How much the current cast prevention was supposed to last
        /// </summary>
        public readonly float castPreventionDuration;
        
        /// <summary>
        /// The remaining cast prevention time 
        /// </summary>
        public readonly float remainingCastPreventionTime;
        
        public CooldownInfo(int charges, int extraCharges, int maxCharges, float ccd, float mxd, float mcdWtcdr, float cpCd, float cCpCd)
        {
            availableCharges = charges;
            this.maxCharges = maxCharges;
            this.extraCharges = extraCharges;
            currentCooldown = ccd;
            maxCooldown = mxd;
            maxCooldownWithoutCdr = mcdWtcdr;

            castPreventionDuration = cpCd;
            remainingCastPreventionTime = cCpCd;
        }
    }

    /// <summary>
    /// Default cooldown handler. Manages cooldown updates with time, as well as
    /// their cooldown reduction and functions to increase or decrease manually the cooldown
    /// of a ability
    /// </summary>
    public class CooldownHandler
    {
        #region CD Helper Class
        protected class CooldownHelper
        {
            /// <summary>
            /// Which ability this CooldownHelper belongs to
            /// </summary>
            public IAbilityBase ability;
            
            /// <summary>
            /// The ability maximum amount of charges
            /// </summary>
            public int maxCharges;
            
            /// <summary>
            /// The ability current amount of charges
            /// </summary>
            public int currentCharges;
            
            /// <summary>
            /// The ability current amount of temporary charges
            /// </summary>
            public int extraCharges;
            
            /// <summary>
            /// The ability current cooldown
            /// </summary>
            public float cooldown;
            
            /// <summary>
            /// The ability current cast prevention cooldown
            /// </summary>
            public float remainingCastPrevention;
            
            /// <summary>
            /// The ability current cast prevention cooldown
            /// </summary>
            public float castPreventionDuration;

            public CooldownHelper(IAbilityBase ability)
            {
                this.ability = ability;
                
                cooldown = 0;
                currentCharges = ability.Charges;
                maxCharges = ability.Charges;
                extraCharges = 0;
                remainingCastPrevention = 0;
                castPreventionDuration = 0;
            }

            public bool HasCharges() => extraCharges + currentCharges > 0;

            public void ConsumeCharge(int charges)
            {
                if (extraCharges > 0)
                {
                    int delta = charges - extraCharges;
                    if (delta < 0)
                        delta = 0;
                    
                    extraCharges -= charges;
                    // if there were more charges to remove than we had temporary charges, we remove the remaining
                    // charges from the current charges
                    currentCharges -= delta;
                }
                else if(currentCharges >= charges)
                {
                    currentCharges -= charges;
                }
            }
        }
        #endregion

        #region Fields
        protected float _globalCdr;
        protected float _maxCdrValue = 0.9f;
        protected CooldownHelper[] _cooldowns;
        protected Dictionary<int, float> _categoriesCdr;
        protected Func<float, float, float> _cdrCalcFunction;
        #endregion


        #region Properties
        public int CooldownsLength => _cooldowns.Length;
        public IReadOnlyDictionary<int, float> CategoriesCDR => _categoriesCdr;

        /// <summary>
        /// Global CDR of abilities. Value can never go below zero.
        /// If a CDR Calculation function wasn't given, the value will be clamped between 0 and Max Cdr Value
        /// </summary>
        public float GlobalCDR
        {
            get => _globalCdr;
            set => _globalCdr = INUMath.Clamp(value, 0, _maxCdrValue);
        }

        /// <summary>
        /// Max CDR value. It is normalized between 0 and 1.
        /// Category CDR + Global CDR, or the function that calculates the total CDR from the Global and
        /// Category CDR will have the values clamped to between 0 and Max CDR Value
        /// </summary>
        public float MaxCdrValue
        {
            get => _maxCdrValue;
            set
            {
                _maxCdrValue = INUMath.Clamp01(value);
                if (_cdrCalcFunction != null)
                    return;

                if (_globalCdr > _maxCdrValue)
                    _globalCdr = _maxCdrValue;

                foreach (int key in _categoriesCdr.Keys.ToList())
                    if (_categoriesCdr[key] > _maxCdrValue)
                        _categoriesCdr[key] = _maxCdrValue;
            }
        }
        #endregion


        #region Constructor
        public CooldownHandler(IAbilityBase[] abilities, Func<float, float, float> cdrCalcFunction = null)
        {
            _cdrCalcFunction = cdrCalcFunction;
            _cooldowns = new CooldownHelper[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i] != null)
                    _cooldowns[i] = new CooldownHelper(abilities[i]);
            }

            _categoriesCdr = new Dictionary<int, float>();
        }
        #endregion


        #region Methods
        /// <summary>
        /// Updates all abilities cooldown
        /// </summary>
        /// <param name="deltaTime">How much time has passed since the last frame</param>
        /// <param name="spellCasted">The ability that is currently being cast by the AbilitiesController</param>
        public void Update(float deltaTime, IAbilityBase spellCasted = null)
        {
            for (uint i = 0; i < _cooldowns.Length; i++)
            {
                if (_cooldowns[i] == null)
                    continue;
                
                IAbilityBase ability = _cooldowns[i].ability;
                if (ability == null || ability == spellCasted)
                    continue;
                
                DecreaseCooldown(i, deltaTime);
                
                _cooldowns[i].remainingCastPrevention -= deltaTime;
                if (_cooldowns[i].remainingCastPrevention < 0)
                {
                    _cooldowns[i].remainingCastPrevention = 0;
                    _cooldowns[i].castPreventionDuration = 0;
                }
            }
        }

        /// <summary>
        /// Tries to get the cooldown of a ability, considering the cooldown reductions
        /// </summary>
        /// <param name="ability">The ability to search for the cooldown</param>
        /// <returns>The given ability cooldown info. Null if ability is not set in the slots</returns>
        public CooldownInfo GetCooldownInfo(IAbilityBase ability)
        {
            int index = Array.FindIndex(_cooldowns, 0, _cooldowns.Length, x => x.ability == ability);
            if (index < 0)
                return null;
            return GetCooldownInfo((uint)index);
        }

        /// <summary>
        /// Returns the cooldon of a ability at a given slot
        /// </summary>
        /// <param name="slot">The ability slot</param>
        /// <returns>The ability cooldown at the given slot. Null if slot is invalid</returns>
        public CooldownInfo GetCooldownInfo(uint slot)
        {
            if (!IsSlotValid(slot))
                return null;

            int charges = _cooldowns[slot].currentCharges;
            int tempCharges = _cooldowns[slot].extraCharges;
            int maxCharges = _cooldowns[slot].maxCharges;
            float cd = _cooldowns[slot].cooldown;
            float totalCd = GetAbilityCooldownWithCdr(slot);
            float totalCdNoCdr = _cooldowns[slot].ability.Cooldown;

            float remainingCastPrevention = _cooldowns[slot].remainingCastPrevention;
            float castPreventionDuration = _cooldowns[slot].castPreventionDuration;

            return new CooldownInfo(
                charges, tempCharges, maxCharges, cd, 
                totalCd, totalCdNoCdr, castPreventionDuration, remainingCastPrevention
            );
        }

        /// <summary>
        /// Tries to put an ability cooldown, by its reference
        /// Considers CooldownHandler Cooldown Reductions.
        /// </summary>
        /// <param name="ability">The ability to put on cooldown</param>
        /// <returns>True if ability exists in the slots and was reset. False otherwise</returns>
        public bool PutOnCooldown(IAbilityBase ability)
        {
            int index = Array.FindIndex(_cooldowns, 0, _cooldowns.Length, x => x != null && x.ability == ability);
            if (index < 0)
                return false;
            return PutOnCooldown((uint)index);
        }

        /// <summary>
        /// Tries to put an ability cooldown through its index and removes an ability charge 
        /// Considers CooldownHandler Cooldown Reductions for the ability cooldown
        /// </summary>
        /// <param name="slot">The index of the ability to reset cooldown</param>
        /// <param name="forceResetCooldown">
        /// Whether or not to force cooldown to be reset when ability is already on cooldown
        /// </param>
        /// <returns>
        /// True if there was an ability in the given slot and it had charges to be cast.
        /// False otherwise
        /// </returns>
        public bool PutOnCooldown(uint slot, bool forceResetCooldown = false)
        {
            bool invalidSlot = slot > _cooldowns.Length || _cooldowns[slot].ability == null;
            if (invalidSlot)
                return false; 

            // don't do anything if the ability is on cooldown and force reset is set to false
            if (IsAbilityOnCd(slot) && !forceResetCooldown)
                return false;
            
            ClampAbilityCooldown(slot, _cooldowns[slot].ability.Cooldown);

            // Updates with a deltaTime of 0 so spells with 0 cooldown won't
            // need to wait until the next frame to be refreshed
            Update(0f);
            return true;
        }

        
        /// <summary>
        /// Puts the ability on cast prevention, disabling casting it at all during this time.
        /// </summary>
        /// <param name="slot">The slot to prevent casting</param>
        /// <param name="duration">How much the cast prevention should last</param>
        /// <returns>
        /// True if the slot was valid and the ability was put on cast prevention. False otherwise
        /// </returns>
        public bool PutOnCastPrevention(uint slot, float duration)
        {
            if (!IsSlotValid(slot) || duration <= 0.001f)
                return false;

            _cooldowns[slot].remainingCastPrevention = duration;
            _cooldowns[slot].castPreventionDuration = duration;
            Update(0f);
            return true;
        }

        /// <summary>
        /// Increases the cooldown of an ability by a given amount, clamping to the max cooldown value,
        /// considering the cooldown reductions
        /// </summary>
        /// <param name="slot">Index of the ability</param>
        /// <param name="amount">how much to increase the cooldown</param>
        /// <returns>
        /// True if the ability was valid and the CD was changed. False otherwise
        /// </returns>
        public bool IncreaseCooldown(uint slot, float amount)
        {
            if (!IsSlotValid(slot) || !IsAbilityOnCd(slot))
                return false;

            ClampAbilityCooldown(slot, _cooldowns[slot].cooldown + amount);
            return true;
        }

        /// <summary>
        /// Decreases the cooldown of an ability by a given amount, clamping to the max cooldown value,
        /// considering the cooldown reductions
        /// </summary>
        /// <param name="slot">Index of the ability</param>
        /// <param name="amount">how much to decrease the cooldown</param>
        /// <returns>True if the ability was valid and the CD was changed. False otherwise</returns>
        public bool DecreaseCooldown(uint slot, float amount)
        {
            if (!IsSlotValid(slot))
                return false;
            
            bool wasOnCooldownOnPreviousFrame = IsAbilityOnCd(slot);
            ClampAbilityCooldown(slot, _cooldowns[slot].cooldown - amount);
            bool isNotCooldownOnThisFrame = !IsAbilityOnCd(slot);
            bool finishedCooldownThisFrame = wasOnCooldownOnPreviousFrame && isNotCooldownOnThisFrame;
            
            bool chargesUpdated = TryAddChargesAfterCDUpdate(slot, finishedCooldownThisFrame);
            bool maxCharges = _cooldowns[slot].currentCharges == _cooldowns[slot].maxCharges;

            if (chargesUpdated && !maxCharges)
            {
                ClampAbilityCooldown(slot, _cooldowns[slot].ability.Cooldown);
            }
            return true;
        }

        /// <summary>
        /// Checks if the ability in the given slot is on cooldown right now
        /// </summary>
        /// <param name="slot">Slot for the ability</param>
        /// <returns>True if on cooldown. False if spell is on cooldown or slot is invalid</returns>
        public bool IsAbilityOnCd(uint slot)
        {
            if (slot < 0 || slot >= _cooldowns.Length)
                return false;

            return _cooldowns[slot].cooldown > 0;
        }

        /// <summary>                                                                                  
        /// Checks if the ability in the given slot is on a cast prevention cooldown right now                           
        /// </summary>                                                                                 
        /// <param name="slot">Slot for the ability</param>                                            
        /// <returns>
        /// True if on cast prevention. False if spell is not on cast prevention cooldown or slot is invalid
        /// </returns>   
        public bool IsAbilityOnCastPrevention(uint slot)
        {
            if (slot < 0 || slot >= _cooldowns.Length)                                         
                return false;                                                                  
                                                                                               
            return _cooldowns[slot].remainingCastPrevention > 0;      
        }

        /// <summary>
        /// Checks if the ability in the given slot has enough charges to be cast
        /// </summary>
        /// <param name="slot">Slot for the ability</param>
        /// <returns>True if has enough charges. False otherwise</returns>
        public bool AbilityHasCharges(uint slot)
        {
            if (!IsSlotValid(slot))
                return false;

            return _cooldowns[slot].currentCharges > 0;
        }

        /// <summary>
        /// Consumes charges from an ability.
        /// </summary>
        /// <param name="slot">The ability slot to remove the charges</param>
        /// <param name="charges">How many charges to remove</param>
        /// <returns>
        /// The new amount of charges an ability have or -1 if no charges were removed due to invalid
        /// slot or not enough charges to consume
        /// </returns>
        public int ConsumeCharges(uint slot, int charges)
        {
            if (charges <= 0 || !IsSlotValid(slot))
                return -1;

            if (_cooldowns[slot].extraCharges + _cooldowns[slot].currentCharges < charges)
                return -1;
            
            _cooldowns[slot].ConsumeCharge(charges);
            return _cooldowns[slot].extraCharges + _cooldowns[slot].currentCharges;
        }

        /// <summary>
        /// Adds available use charges to an Ability. The amount won't go past
        /// the maximum amount of charges.
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <param name="amount">how many charges to add</param>
        /// <returns>True if there was an ability to add charges to at the given slot. False otherwise</returns>
        public bool AddAvailableAbilityCharges(uint slot, int amount)
        {
            if (!IsSlotValid(slot))
                return false;

            CooldownHelper cd = _cooldowns[slot];
            _cooldowns[slot].currentCharges = INUMath.Clamp(cd.currentCharges + amount, 0, cd.maxCharges);
            return true;
        }

        /// <summary>
        /// Removes the given amount of available charges from an ability. It can't go
        /// below 0 charges.
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <param name="amount">How many charges to remove</param>
        /// <returns>
        /// The new amount of charges if there was an ability to remove charges from at the given slot.
        /// -1 if there were no valid ability at the given slot
        /// </returns>
        public int RemoveAvailableCharges(uint slot, int amount)
        {
            if (!IsSlotValid(slot))
                return -1;

            CooldownHelper cd = _cooldowns[slot];
            _cooldowns[slot].currentCharges = INUMath.Clamp(cd.currentCharges - amount, 0, cd.maxCharges);
            return _cooldowns[slot].currentCharges;
        }

        /// <summary>
        /// Sets the max amount of charges an ability can have.
        /// If the amount of charges are less than the current ability charges,
        /// it will loose charges to compensate
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <param name="size">The amount of max charges the ability will have</param>
        /// <param name="addChargesIfAbilityIsAtMaxCharges">
        /// Whether or not to add charges to an ability if the ability was at max charges
        /// </param>
        /// <returns>
        /// True if the ability had the max charges updated.
        /// False otherwise or if there was no ability in the given slot.
        /// </returns>
        public bool SetAbilityMaxCharges(uint slot, int size, bool addChargesIfAbilityIsAtMaxCharges)
        {
            if (!IsSlotValid(slot))
                return false;

            int clampedSize = size < 0 ? 0 : size;
            int prevMaxSize = _cooldowns[slot].maxCharges;

            if (clampedSize < prevMaxSize)
            {
                int diff = prevMaxSize - clampedSize;
                _cooldowns[slot].currentCharges -= diff;

                if (_cooldowns[slot].currentCharges < 0)
                    _cooldowns[slot].currentCharges = 0;
            }

            _cooldowns[slot].maxCharges = clampedSize;
            if (addChargesIfAbilityIsAtMaxCharges && _cooldowns[slot].currentCharges == prevMaxSize)
                _cooldowns[slot].currentCharges = clampedSize;

            return true;
        }

        /// <summary>
        /// Adds extra charges to an ability. Those charges are bonus
        /// and thus can go above the max charges.
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <param name="amount">The amount of temporary charges to be added</param>
        /// <returns></returns>
        public bool AddExtraAbilityCharges(uint slot, int amount)
        {
            if (!IsSlotValid(slot))
                return false;

            _cooldowns[slot].extraCharges += amount;
            return true;
        }

        /// <summary>
        /// Removes all temporary charges for an ability
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <returns>True if the temporary charges was removed. False otherwise</returns>
        public bool RemoveAllAbilityExtraCharges(uint slot)
        {
            if (!IsSlotValid(slot))
                return false;

            _cooldowns[slot].extraCharges = 0;
            return true;
        }
        
        /// <summary>
        /// Removes the given amount of temporary charges for an ability
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <param name="amount">how many charges to remove</param>
        /// <returns>
        /// The new amount of temporary charges of the ability.
        /// -1 if there were no valid abilities at the given slot
        /// </returns>
        public int ConsumeExtraCharges(uint slot, int amount)
        {
            if (!IsSlotValid(slot))
                return -1;

            _cooldowns[slot].extraCharges -= amount;
            if (_cooldowns[slot].extraCharges < 0)
                _cooldowns[slot].extraCharges = 0;
            
            return _cooldowns[slot].extraCharges;
        }

        /// <summary>
        /// Sets the amount of cooldown reduction for a ability category
        /// </summary>
        /// <param name="category">The ability category</param>
        /// <param name="cdr">The new CDR amount</param>
        public void SetCategoryCdr(int category, float cdr)
        {
            if (_categoriesCdr.ContainsKey(category))
                _categoriesCdr[category] = cdr;
            else
                _categoriesCdr.Add(category, cdr);

            _categoriesCdr[category] = INUMath.Clamp(_categoriesCdr[category], 0, _maxCdrValue);
        }


        /// <summary>
        /// Sums an amount of CDR to the ability CDR category
        /// </summary>
        /// <param name="category">The ability category</param>
        /// <param name="cdr">The CDR amount to sum</param>
        public void AddCategoryCdr(int category, float cdr)
        {
            if (_categoriesCdr.ContainsKey(category))
                _categoriesCdr[category] += cdr;
            else
                _categoriesCdr.Add(category, cdr);

            _categoriesCdr[category] = INUMath.Clamp(_categoriesCdr[category], 0, _maxCdrValue);
        }

        /// <summary>
        /// Gets the amount of CDR for the given ability category
        /// </summary>
        /// <param name="category">The ability category</param>
        /// <returns>The current ability CDR for the given category. 0 if the category doesn't exist</returns>
        public float GetCategoryCdr(int category)
        {
            return _categoriesCdr.ContainsKey(category) ? _categoriesCdr[category] : 0;
        }

        /// <summary>
        /// Sets the CDR Calculation function.
        /// Returns a float, from the Global CDR and Category CDR, respectively.
        /// The value will be clamped between 0-MaxCdr.
        /// </summary>
        /// <param name="func">
        /// The new CDR calculation function. If the function is null, the default function will be used.
        /// The default function simply sums GlobalCDR and CategoryCdr.
        /// </param>
        public void SetCdrFunction(Func<float, float, float> func) => _cdrCalcFunction = func;
        #endregion


        #region Helper Methods
        public void SetAbility(uint slot, IAbilityBase ability)
        {
            if (ability == null)
                _cooldowns[slot] = null;
            else
                _cooldowns[slot] = new CooldownHelper(ability);
        }

        protected bool ClampAbilityCooldown(uint slot, float newValue)
        {
            if (!IsSlotValid(slot))
                return false;

            float maxCooldown = GetAbilityCooldownWithCdr(slot);
            _cooldowns[slot].cooldown = INUMath.Clamp(newValue, 0, maxCooldown);

            return true;
        }

        protected float GetAbilityCooldownWithCdr(uint slot)
        {
            float categoryCdr = GetCategoryCdr(_cooldowns[slot].ability.Category);
            float maxCooldown;
            float cd = _cooldowns[slot].ability.Cooldown;
            if (_cdrCalcFunction == null)
                maxCooldown = cd * (1 - INUMath.Clamp(GlobalCDR + categoryCdr, 0, _maxCdrValue));
            else
                maxCooldown = cd * (1 - INUMath.Clamp(_cdrCalcFunction(GlobalCDR, categoryCdr), 0, _maxCdrValue));

            return maxCooldown;
        }

        protected bool TryAddChargesAfterCDUpdate(uint slot, bool finishedCooldownThisFrame)
        {
            CooldownHelper cd = _cooldowns[slot];
                        
            bool canAddMoreCharges = cd.currentCharges < cd.maxCharges;
            bool shouldUpdateCharges = canAddMoreCharges && finishedCooldownThisFrame;
            if (shouldUpdateCharges)
            {
                cd.currentCharges = INUMath.Clamp(cd.currentCharges + 1, 0, cd.maxCharges);
            }

            return shouldUpdateCharges;
        }

        protected bool IsSlotValid(uint slot) => 
            !(
              slot < 0 || slot >= _cooldowns.Length || 
              _cooldowns[slot] == null || _cooldowns[slot].ability == null
            );

    #endregion
    }
}