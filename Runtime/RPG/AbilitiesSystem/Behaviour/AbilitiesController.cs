using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Default controller for agents that can use the ability system
    /// Fully manages the abilities cooldown and their casting process
    /// </summary>
    public class AbilitiesController<TAbility, TCaster> 
           where TAbility : class, IAbility<TCaster> where TCaster : IAbilityCaster
    {
        #region Fields
        protected TCaster _caster;
        protected TAbility[] _abilities;
        protected TAbility _casting;
        protected float _elapsedChanneling;
        protected CastingState _castingState;
        protected CooldownHandler _cdHandler;
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
        /// 
        /// </summary>
        public CastingState CastingState => _castingState;
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt, TCaster caster)
        {
            AbilitySlots = slotAmnt;
            _caster = caster;
            _abilities = new TAbility[slotAmnt];
            _casting = null;
            _cdHandler = new CooldownHandler(_abilities);
        }
        #endregion


        #region Methods
        /// <summary>
        /// Function to run in the Update of a MonoBehaviour
        /// Updates each ability current CD
        /// </summary>
        /// <param name="deltaTime">How much time elapsed since the last frame</param>
        public virtual void Update(float deltaTime)
        {
            _cdHandler.Update(deltaTime);

            if(_casting != null && _castingState == CastingState.Channeling)
            {
                _elapsedChanneling += deltaTime;
                if(_elapsedChanneling >= _casting.ChannelingTime)
                    UnleashAbility();
            }
        }

        /// <summary>
        /// Starts channeling the ability in the given slot
        /// If the channeling time is 0, the ability is unleashed
        /// instantly
        /// </summary>
        /// <param name="slot">The ability slot index</param>
        public virtual void StartChanneling(uint slot)
        {
            if(HasAbilityInSlot(slot) && !_cdHandler.IsAbilityOnCd((int)slot) && _casting == null)
            {
                _casting = _abilities[slot];
                _casting.OnChannelingStarted(_caster);
                _castingState = CastingState.Channeling;

                // If the cast time for the spell is zero, 
                // just cast it instantly
                if(_casting.ChannelingTime == 0f)
                    UnleashAbility();
            }
        }

        /// <summary>
        /// Cancels the ability channeling for the current spell,
        /// reseting the ElapsedCasting timer and setting the casting spell to null
        /// </summary>
        public virtual void CancelChanneling() 
        {
            _casting.OnChannelingCanceled(_caster);
            _casting = null;

            _elapsedChanneling = 0f;
            _castingState = CastingState.None;
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
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to check if there's an ability in the given slot
        /// </summary>
        /// <param name="slot">The slot index</param>
        /// <returns>True if there's an ability in the slot. False othwerwise</returns>
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;

        /// <summary>
        /// Helper method to cast the ability after it is ready to be cast
        /// </summary>
        protected void UnleashAbility()
        {
            _castingState = CastingState.Casting;

            _casting.OnChannelingCompleted(_caster);
            _cdHandler.ResetCooldown(_casting);
            _casting.Cast(_caster, FinishAbilityCasting);

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
        }
        #endregion
    }
}