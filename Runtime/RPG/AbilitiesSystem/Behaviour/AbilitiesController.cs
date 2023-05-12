using System;
using System.Collections.Generic;
using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Default controller for agents that can use the ability system
    /// Fully manages the abilities cooldown and their casting process
    /// </summary>
    public class AbilitiesController<TAbility, TCaster> 
           where TAbility : class, IAbility<TCaster> where TCaster : ICasterInfo
    {
        #region Fields
        protected TCaster _caster;
        protected TAbility[] _abilities;
        protected TAbility _casting;
        protected float _elapsedChanneling;
        protected CastingState _castingState;
        protected CooldownHandler _cdHandler;
        protected CastHandler _castHandler;
        protected List<CastHandler> _activeAbilities;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the elapsed cast time for the spell being charged.
        /// Will return 0 if no spell is being cast;
        /// </summary>
        public float ElapsedChannelingTime => _elapsedChanneling;

        /// <summary>
        /// Returns the number of AbilitySlots assigned in the constructor
        /// </summary>
        public uint AbilitySlots {get; private set;}

        /// <summary>
        /// Returns the current cooldown handler that manages everything related to abilities cooldowns
        /// </summary>
        public CooldownHandler CooldownsHandler => _cdHandler;

        /// <summary>
        /// Getter for the AbilityController Data Hub
        /// </summary>
        public TCaster DataHub => _caster;

        /// <summary>
        /// Getter for the current actor casting state
        /// </summary>
        public CastingState CastingState => _castingState;

        public IReadOnlyList<CastHandler> ActiveAbilities => _activeAbilities;
        
        public CooldownUpdateType CooldownUpdateType { get; set; }
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt, TCaster caster, Func<float, float, float> cdrCalcFunc = null)
        {
            AbilitySlots = slotAmnt;
            _caster = caster;
            _abilities = new TAbility[slotAmnt];
            _casting = null;
            _activeAbilities = new List<CastHandler>();
            _cdHandler = new CooldownHandler(_abilities, cdrCalcFunc);
        }
        #endregion


        #region Cast Methods
        /// <summary>
        /// Starts channeling the ability in the given slot
        /// If the channeling time is 0, the ability is unleashed
        /// instantly
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        /// <returns>True if the ability started channeling. False otherwise</returns>
        public bool StartChanneling(uint slot)
        {
            if(!HasAbilityInSlot(slot) || !_abilities[slot].CanCast(_caster))
                return false;

            if(!_cdHandler.IsAbilityOnCd(slot) && _casting == null)
            {
                _casting = _abilities[slot];
                _castingState = CastingState.Channeling;
                CastObjects castInfo = _casting.Cast(_caster);

                var handler = new CastHandler(castInfo, GetCastingState);
                _castHandler = handler;
                _activeAbilities.Add(_castHandler);

                castInfo.abilityObject.Disable();
                castInfo.abilityObject.NotifyFinishCast += FinishCastAbilityCasting;
                castInfo.abilityObject.NotifyDiscard += () => RemoveAbility(handler);

                // If the cast time for the spell is zero, 
                // just cast it instantly
                if(_casting.ChannelingTime == 0f)
                    UnleashAbility();
                
                // checking if it's equal to 0 with a range to avoid floating point imprecision 
                bool equalsZero = _casting.RecoveryTime < 0.001f && _casting.RecoveryTime > -0.001f;
                bool isFireAndForget = _casting.AbilityCastType == AbilityCastType.FireAndForget;
                if (equalsZero && isFireAndForget)
                {
                    _castHandler.AbilityObject.EndAbilityObject();
                }
            }
            else if(_casting == GetAbility(slot))
            {
                _castHandler.OnCast();
            }

            return true;
        }

        /// <summary>
        /// Cancels the ability channeling for the current spell,
        /// reseting the ElapsedCasting timer and setting the casting spell to null
        /// </summary>
        public virtual void CancelCast() 
        {
            if (_castingState == CastingState.Channeling)
            {
                _castHandler?.OnCastCanceled();
                
            }
            else if (_castingState == CastingState.Casting)
            {
                _castHandler.AbilityObject.Cancel();
            }
            
            _casting = null;
            _castHandler = null;
            _elapsedChanneling = 0f;
            _castingState = CastingState.None;
        } 

        /// <summary>
        /// Helper method to cast the ability after it is ready to be cast
        /// </summary>
        protected virtual void UnleashAbility()
        {
            _castingState = CastingState.Casting;
            _cdHandler.PutOnCooldown(_casting);
            _castHandler.AbilityObject.UnleashAbility();

            _elapsedChanneling = 0f;
        }

        /// <summary>
        /// Finishes casting the current ability, setting the Casted Ability to null and
        /// the casting state to None
        /// </summary>        
        public void FinishCastAbilityCasting()
        {
            _castingState = CastingState.None;
            _casting = null;
            _castHandler = null;
        }

        public void RemoveAbility(CastHandler abilityObject) => _activeAbilities.Remove(abilityObject);
        #endregion


        #region Methods
        /// <summary>
        /// Function to run in the Update of a MonoBehaviour
        /// Updates each ability current CD
        /// </summary>
        /// <param name="deltaTime">How much time elapsed since the last frame</param>
        public virtual void UpdateCooldowns(float deltaTime) => _cdHandler.Update(deltaTime);

        public virtual void Update(float deltaTime)
        {
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].Update(deltaTime);

            if (CooldownUpdateType == CooldownUpdateType.Auto)
                UpdateCooldowns(deltaTime);

            if(_casting != null)
            {
                if (_castingState == CastingState.Channeling)
                {
                    _elapsedChanneling += deltaTime;
                    if(_elapsedChanneling >= _casting.ChannelingTime)
                        UnleashAbility();
                }
                else if (_castingState == CastingState.Casting && _casting.RecoveryTime > 0)
                {
                    _elapsedChanneling += deltaTime;
                    if (_elapsedChanneling >= _casting.RecoveryTime && _casting.AbilityCastType == AbilityCastType.FireAndForget)
                    {
                        _castHandler.AbilityObject.EndAbilityObject();
                    }
                }
            }
        }

        public void OnDrawGizmos()
        {
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].DrawGizmos();
        }
    
        /// <summary>
        /// Sets the ability on the given index
        /// </summary>
        /// <param name="slot">Slot to set the ability</param>
        /// <param name="ability">The ability that is being added</param>
        public void SetAbility(uint slot, TAbility ability) => _abilities[slot] = ability;

        /// <summary>
        /// Retrieves an ability from the abilities slots
        /// </summary>
        /// <param name="slot">slot index</param>
        /// <returns>The ability in the slot. Null if no spell in slot or slot have invalid index</returns>
        public TAbility GetAbility(uint slot) 
        {
            if(HasAbilityInSlot(slot))
                return _abilities[slot];
        
            return null;
        }

        /// <summary>
        /// Gets the ability that is currently being cast
        /// </summary>
        /// <returns>The ability being cast. Null if no ability is being cast</returns>
        public TAbility GetCastingAbility() => _casting;
        public CastHandler GetCastHandler() => _castHandler;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to check if there's an ability in the given slot
        /// </summary>
        /// <param name="slot">The slot index</param>
        /// <returns>True if there's an ability in the slot. False othwerwise</returns>
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;

        /// <summary>
        /// Helper method to provide a getter function to pass to the cast handler
        /// </summary>
        /// <returns>The Controller cast state</returns>
        private CastingState GetCastingState() => CastingState;
        #endregion
    }
}