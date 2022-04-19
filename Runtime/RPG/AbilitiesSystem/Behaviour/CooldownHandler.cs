using System;
using System.Collections.Generic;
using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Struct containing all cooldown info for the ability.
    /// Current Cooldown, Total Cooldown and Total Coodlwon without the Cooldown reduction
    /// </summary>
    public class CooldownInfo
    {
        public readonly float currentCooldown;
        public readonly float totalCooldown;
        public readonly float totalCooldownWithoutCdr;

        public CooldownInfo(float ccd, float tcd, float tcdwcdr)
        {
            currentCooldown = ccd;
            totalCooldown = tcd;
            totalCooldownWithoutCdr = tcdwcdr;
        }
    }

    /// <summary>
    /// Default cooldown handler. Manages cooldown updates with time, as well as
    /// their cooldown reduction and functions to increase or decrease manually the cooldown
    /// of a ability
    /// </summary>
    public class CooldownHandler 
    {
        #region Fields
        protected float _globalCdr;
        protected float _maxCdrValue = 0.9f;
        protected float[] _cooldowns;
        protected IAbilityBase[] _abilities;
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
            set => _globalCdr = Mathf.Clamp(value, 0, _maxCdrValue);
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
                _maxCdrValue = Mathf.Clamp01(value);
                if(_cdrCalcFunction != null)
                    return;

                if(_globalCdr > _maxCdrValue)
                    _globalCdr = _maxCdrValue;

                foreach(var cdr in _categoriesCdr)
                    if(cdr.Value > _maxCdrValue)
                        _categoriesCdr[cdr.Key] = _maxCdrValue;
            }
        }
        #endregion


        #region Constructor
        public CooldownHandler(IAbilityBase[] abilities, Func<float, float, float> cdrCalcFunction = null)
        {
            _abilities = abilities;
            _cdrCalcFunction = cdrCalcFunction;
            _cooldowns = new float[abilities.Length];
            _categoriesCdr = new Dictionary<int, float>();
        } 
        #endregion


        #region Methods
        /// <summary>
        /// Updates the 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime, IAbilityBase spellCasted = null)
        {
            for(uint i = 0; i < _abilities.Length; i++)
            {
                IAbilityBase ability = _abilities[i];
                if(ability == null || ability == spellCasted)
                    continue;

                DecreaseCooldown(i, deltaTime);
            }
        }

        /// <summary>
        /// Tries to get the cooldown of a ability, considering the cooldown reductions
        /// </summary>
        /// <param name="ability">The ability to search for the cooldown</param>
        /// <returns>The given ability cooldown info. Null if ability is not set in the slots</returns>
        public CooldownInfo GetCooldownInfo(IAbilityBase ability)
        {
            int index = Array.FindIndex(_abilities, 0, _abilities.Length, x => x == ability);
            if(index < 0)
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
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return null;
            
            float cd = _cooldowns[slot];
            float totalCd = GetAbilityCooldownWithCdr(slot);
            float totalCdNoCdr = _abilities[slot].Cooldown;

            return new CooldownInfo(cd, totalCd, totalCdNoCdr);
        }

        /// <summary>
        /// Tries to reset an ability cooldown, by its reference
        /// Considers CooldownHandler Cooldown Reductions.
        /// </summary>
        /// <param name="ability">The ability to reset the cooldown</param>
        /// <returns>True if ability exists in the slots and was reset. False otherwise</returns>
        public bool ResetCooldown(IAbilityBase ability)
        {
            int index = Array.FindIndex(_abilities, 0, _abilities.Length, x => x == ability);
            if(index < 0)
                return false;
            return ResetCooldown((uint)index);
        }

        /// <summary>
        /// Tries to reset an ability cooldown through its index. 
        /// Considers CooldownHandler Cooldown Reductions.
        /// </summary>
        /// <param name="slot">The ability to reset the cooldown</param>
        /// <returns>True if there was an ability in the given slot. False otherwise</returns>
        public bool ResetCooldown(uint slot)
        {
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return false;
            
            ClampAbilityCooldown(slot, _abilities[slot].Cooldown);
            return true;
        }

        /// <summary>
        /// Increases the cooldown of an ability by a given amount, clamping to the max cooldown value,
        /// considering the cooldown reductions
        /// </summary>
        /// <param name="slot">Index of the ability</param>
        /// <param name="amount">how much to increase the cooldown</param>
        /// <returns>True if the ability was valid and the CD was changed. False otherwise<returns>
        public bool IncreaseCooldown(uint slot, float amount)
        {
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return false;

            ClampAbilityCooldown(slot, _cooldowns[slot] + amount);
            return true;
        }

        /// <summary>
        /// Decreases the cooldown of an ability by a given amount, clamping to the max cooldown value,
        /// considering the cooldown reductions
        /// </summary>
        /// <param name="slot">Index of the ability</param>
        /// <param name="amount">how much to decrease the cooldown</param>
        /// <returns>True if the ability was valid and the CD was changed. False otherwise<returns>
        public bool DecreaseCooldown(uint slot, float amount)
        {
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return false;

            ClampAbilityCooldown(slot, _cooldowns[slot] - amount);
            return true;
        }

        /// <summary>
        /// Checks if the ability in the given slot is on cooldown right now
        /// </summary>
        /// <param name="slot">Slot for the ability</param>
        /// <returns>True if on cooldown. False if spell is on cooldown or slot is invalid</returns>
        public bool IsAbilityOnCd(uint slot)
        {
            if(slot < 0 || slot >= _cooldowns.Length)
                return false;
            
            return _cooldowns[slot] > 0.001f;
        }

        /// <summary>
        /// Sets the amount of cooldown reduction for a ability category
        /// </summary>
        /// <param name="category">The ability category</param>
        /// <param name="cdr">The new CDR amount</param>
        public void SetCategoryCdr(int category, float cdr)
        {
            if(_categoriesCdr.ContainsKey(category))
                _categoriesCdr[category] = cdr;
            else
                _categoriesCdr.Add(category, cdr);

            _categoriesCdr[category] = Mathf.Clamp(_categoriesCdr[category], 0, _maxCdrValue);
        }


        /// <summary>
        /// Sums an amount of CDR to the ability CDR category
        /// </summary>
        /// <param name="category">The ability category</param>
        /// <param name="cdr">The CDR amount to sum</param>
        public void AddCategoryCdr(int category, float cdr)
        {
            if(_categoriesCdr.ContainsKey(category))
                _categoriesCdr[category] += cdr;
            else
                _categoriesCdr.Add(category, cdr);

            _categoriesCdr[category] = Mathf.Clamp(_categoriesCdr[category], 0, _maxCdrValue);
        }

        /// <summary>
        // Gets the amount of CDR for the given ability category
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
        protected bool ClampAbilityCooldown(uint slot, float newValue)
        {
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return false;

            float maxCooldown = GetAbilityCooldownWithCdr(slot);
            _cooldowns[slot] = Mathf.Clamp(newValue, 0, maxCooldown);

            return true;
        }

        protected float GetAbilityCooldownWithCdr(uint slot)
        {
            float categoryCdr = GetCategoryCdr(_abilities[slot].Category);
            float maxCooldown;
            float cd = _abilities[slot].Cooldown;
            if(_cdrCalcFunction == null)
                maxCooldown = cd * (1 - Mathf.Clamp(GlobalCDR + categoryCdr, 0, _maxCdrValue));
            else
                maxCooldown = cd * (1 - Mathf.Clamp(_cdrCalcFunction(GlobalCDR, categoryCdr), 0, _maxCdrValue));

            return maxCooldown;
        }
        #endregion
    }
}