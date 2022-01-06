using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    public class AbilitiesController
    {
        #region Fields
        protected IAbility[] _abilities;
        protected IAbility _casting;
        protected float _elapsedCasting;
        #endregion

        #region Properties
        public float ElapsedCastingTime => _elapsedCasting;
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt)
        {
            _abilities = new IAbility[slotAmnt];
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
            if(HasAbilityInSlot(slot) && !IsAbilityOnCd(0) && _casting == null)
            {
                _casting = _abilities[slot];
                // If the cast time for the spell is zero, 
                // just cast it instantly
                if(_casting.CastTime == 0f)
                    UnleashAbility();
            }
        }

        public virtual void CancelCast() => _casting = null;

        /// <summary>
        /// Sets the ability on the given index
        /// </summary>
        /// <param name="slot">Slot to set the ability</param>
        /// <param name="ability">The ability that is being added</param>
        public void SetAbility(uint slot, IAbility ability) => _abilities[slot] = ability;

        public IAbility GetAbility(int slot) => _abilities[slot];
        public bool IsAbilityOnCd(uint slot) => HasAbilityInSlot(slot) && _abilities[slot].CurrentCooldown > 0;
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;
        public IAbility GetCastingAbility() => _casting;
        #endregion


        #region Helper Methods
        protected void UnleashAbility()
        {
            _casting.Cast();
            _casting.CurrentCooldown = _casting.Cooldown;
            _casting = null;
            _elapsedCasting = 0f;
        }
        #endregion
    }
}