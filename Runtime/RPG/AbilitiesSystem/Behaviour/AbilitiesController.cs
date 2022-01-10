using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Default controller for agents that can use the ability system
    /// Fully manages the abilities cooldown and their casting process
    /// </summary>
    public class AbilitiesController<TAbility, TAbilityDataDactory> 
           where TAbility : class, IAbility<TAbilityDataDactory> where TAbilityDataDactory : IAbilityDataFactory
    {
        #region Fields
        protected TAbilityDataDactory _dataFactory;
        protected TAbility[] _abilities;
        protected TAbility _casting;
        protected float _elapsedCasting;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the elapsed cast time for the spell being charged.
        /// Will return 0 if no spell is being cast;
        /// </summary>
        public float ElapsedCastingTime => _elapsedCasting;

        /// <summary>
        /// Returns the number of AbilitySlots assigned in the constructor
        /// </summary>
        public uint AbilitySlots {get; private set;}

        public TAbilityDataDactory DataFactory => _dataFactory;
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt, TAbilityDataDactory dataFactory)
        {
            AbilitySlots = slotAmnt;
            _dataFactory = dataFactory;
            _abilities = new TAbility[slotAmnt];
            _casting = null;
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
            foreach(var ability in _abilities)
            {
                if(ability != null && ability.CurrentCooldown >= 0.0f && ability != _casting) {
                    ability.CurrentCooldown -= deltaTime;
                    ability.CurrentCooldown = Mathf.Clamp(ability.CurrentCooldown, 0, ability.Cooldown);
                }
            }

            if(_casting != null)
            {
                _elapsedCasting += deltaTime;
                if(_elapsedCasting >= _casting.CastTime)
                    UnleashAbility();
            }
        }

        /// <summary>
        /// Casts the ability in the given slot
        /// </summary>
        /// <param name="slot"></param>
        public virtual void StartCast(uint slot)
        {
            if(HasAbilityInSlot(slot) && !IsAbilityOnCd(slot) && _casting == null)
            {
                _casting = _abilities[slot];
                // If the cast time for the spell is zero, 
                // just cast it instantly
                if(_casting.CastTime == 0f)
                    UnleashAbility();
            }
        }

        /// <summary>
        /// Cancels the cast charge for the current spell,
        /// reseting the ElapsedCasting timer and setting the casting spell to null
        /// </summary>
        public virtual void CancelCast() {
            _elapsedCasting = 0f;
            _casting = null;
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
        /// Checks if the ability in the given slot is on cooldown right now
        /// </summary>
        /// <param name="slot">Slot for the ability</param>
        /// <returns>True if on cooldown. False if spell is on cooldown or slot is invalid</returns>
        public bool IsAbilityOnCd(uint slot) => HasAbilityInSlot(slot) && _abilities[slot].CurrentCooldown > 0;

        /// <summary>
        /// Helper method to check if there's an ability in the given slot
        /// </summary>
        /// <param name="slot">The slot index</param>
        /// <returns>True if there's an ability in the slot. False othwerwise</returns>
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;

        /// <summary>
        /// Gets the ability that is currently being cast
        /// </summary>
        /// <returns>The ability being cast. Null if no ability is being cast</returns>
        public TAbility GetCastingAbility() => _casting;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to cast the ability after it is ready to be cast
        /// </summary>
        protected void UnleashAbility()
        {
            _casting.Cast(_dataFactory);
            _casting.CurrentCooldown = _casting.Cooldown;
            _casting = null;
            _elapsedCasting = 0f;
        }
        #endregion
    }
}