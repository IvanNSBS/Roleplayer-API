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
        #endregion


        #region Properties
        public int CooldownsLength => _cooldowns.Length;
        public IReadOnlyDictionary<int, float> CategoriesCDR => _categoriesCdr;

        /// <summary>
        /// Global CDR of abilities. Value is normalized
        /// </summary>
        public float GlobalCDR 
        {
            get => _globalCdr;
            set => _globalCdr = Mathf.Clamp(value, 0, _maxCdrValue);
        }

        /// <summary>
        /// Max CDR value. It is normalized
        /// </summary>
        /// <value></value>
        public float MaxCdrValue
        {
            get => _maxCdrValue;
            set 
            {
                _maxCdrValue = Mathf.Clamp01(value);
                if(_globalCdr > _maxCdrValue)
                    _globalCdr = _maxCdrValue;
            }
        }
        #endregion


        #region Constructor
        public CooldownHandler(IAbilityBase[] abilities)
        {
            _abilities = abilities;
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
            for(int i = 0; i < _abilities.Length; i++)
            {
                IAbilityBase ability = _abilities[i];
                if(ability == null || ability == spellCasted)
                    continue;

                DecreaseCooldown(i, deltaTime);
            }
        }

        /// <summary>
        /// Tries to get the cooldown of a ability.
        /// </summary>
        /// <param name="ability">The ability to search for the cooldown</param>
        /// <returns>The given ability cooldown info. Null if ability is not set in the slots</returns>
        public CooldownInfo GetCooldownInfo(IAbilityBase ability)
        {
            int index = Array.FindIndex(_abilities, 0, _abilities.Length, x => x == ability);
            return GetCooldownInfo(index);
        }

        /// <summary>
        /// Returns the cooldon of a ability at a given slot
        /// </summary>
        /// <param name="slot">The ability slot</param>
        /// <returns>The ability cooldown at the given slot. Null if slot is invalid</returns>
        public CooldownInfo GetCooldownInfo(int slot)
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
            return ResetCooldown(index);
        }

        /// <summary>
        /// Tries to reset an ability cooldown through its index. 
        /// Considers CooldownHandler Cooldown Reductions.
        /// </summary>
        /// <param name="slot">The ability to reset the cooldown</param>
        /// <returns>True if there was an ability in the given slot. False otherwise</returns>
        public bool ResetCooldown(int slot)
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
        public bool IncreaseCooldown(int slot, float amount)
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
        public bool DecreaseCooldown(int slot, float amount)
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
        public bool IsAbilityOnCd(int slot)
        {
            if(slot < 0 || slot >= _cooldowns.Length)
                return false;
            
            return _cooldowns[slot] > 0.001f;
        }
        #endregion


        #region Helper Methods
        protected bool ClampAbilityCooldown(int slot, float newValue)
        {
            if(slot < 0 || slot > _cooldowns.Length || _abilities[slot] == null)
                return false;

            float maxCooldown = GetAbilityCooldownWithCdr(slot);
            _cooldowns[slot] = Mathf.Clamp(newValue, 0, maxCooldown);

            return true;
        }

        protected float GetAbilityCooldownWithCdr(int slot)
        {
            float categoryCdr = 0;
            float maxCooldown = _abilities[slot].Cooldown * (1 - GlobalCDR + categoryCdr);

            return maxCooldown;
        }
        #endregion
    }
}