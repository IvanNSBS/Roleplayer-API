using System;
using System.Collections.Generic;

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
        protected CastHandler<TAbility, TCaster> _castHandler;
        protected List<IAbilityObject> _activeAbilities;
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

        public IReadOnlyList<IAbilityObject> ActiveObjects => _activeAbilities;
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt, TCaster caster, Func<float, float, float> cdrCalcFunc = null)
        {
            AbilitySlots = slotAmnt;
            _caster = caster;
            _abilities = new TAbility[slotAmnt];
            _casting = null;
            _activeAbilities = new List<IAbilityObject>();
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
        public virtual void StartChanneling(uint slot)
        {
            if(!HasAbilityInSlot(slot))
                return;

            if(!_cdHandler.IsAbilityOnCd(slot) && _casting == null)
            {
                _casting = _abilities[slot];
                _castingState = CastingState.Channeling;
                CastObjects castInfo = _casting.Cast(_caster);

                _castHandler = new CastHandler<TAbility, TCaster>(this, _caster, castInfo);
                _activeAbilities.Add(castInfo.abilityObject);

                castInfo.abilityObject.Disable();
                castInfo.abilityObject.OnFinishCast += FinishAbilityCasting;
                castInfo.abilityObject.OnAbilityFinished += () => RemoveAbility(castInfo.abilityObject);

                // If the cast time for the spell is zero, 
                // just cast it instantly
                if(_casting.ChannelingTime == 0f)
                    UnleashAbility();
            }
            else if(_casting == GetAbility(slot))
            {
                _castHandler.OnCast();
            }
        }

        /// <summary>
        /// Cancels the ability channeling for the current spell,
        /// reseting the ElapsedCasting timer and setting the casting spell to null
        /// </summary>
        public virtual void CancelChanneling() 
        {
            _castHandler?.OnCastCanceled();

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
            _cdHandler.ResetCooldown(_casting);
            _castHandler.AbilityObject.UnleashAbility();

            _elapsedChanneling = 0f;
        }

        /// <summary>
        /// Finishes casting the current ability, setting the Casted Ability to null and
        /// the casting state to None
        /// </summary>        
        public void FinishAbilityCasting()
        {
            _castingState = CastingState.None;
            _casting = null;
            _castHandler = null;
        }

        public void RemoveAbility(IAbilityObject abilityObject) => _activeAbilities.Remove(abilityObject);
        #endregion


        #region Methods
        /// <summary>
        /// Function to run in the Update of a MonoBehaviour
        /// Updates each ability current CD
        /// </summary>
        /// <param name="deltaTime">How much time elapsed since the last frame</param>
        public virtual void UpdateCDHandler(float deltaTime) => _cdHandler.Update(deltaTime);

        public virtual void Update(float deltaTime)
        {
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].OnUpdate(deltaTime);

            // _cdHandler.Update(deltaTime);

            if(_casting != null && _castingState == CastingState.Channeling)
            {
                _elapsedChanneling += deltaTime;
                if(_elapsedChanneling >= _casting.ChannelingTime)
                    UnleashAbility();
            }
        }

        public void OnDrawGizmos()
        {
            for(int i = _activeAbilities.Count - 1; i >= 0; i--)
                _activeAbilities[i].OnDrawGizmos();
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
        public CastHandler<TAbility, TCaster> GetCastHandler() => _castHandler;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to check if there's an ability in the given slot
        /// </summary>
        /// <param name="slot">The slot index</param>
        /// <returns>True if there's an ability in the slot. False othwerwise</returns>
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;
        #endregion
    }
}