using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    public class AbilitiesController
    {
        #region Fields
        protected IAbility[] _abilities;
        #endregion

        #region Properties
        #endregion


        #region Constructor
        public AbilitiesController(uint slotAmnt)
        {
            _abilities = new IAbility[slotAmnt];
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
                if(ability != null)
                    ability.CurrentCooldown -= deltaTime;
            }
        }

        /// <summary>
        /// Casts the ability in the given slot
        /// </summary>
        /// <param name="slot"></param>
        public virtual void Cast(uint slot)
        {
            if(HasAbilityInSlot(slot) && !IsAbilityOnCd(0))
            {
                var ability = _abilities[slot];
                ability.Cast();
                ability.CurrentCooldown = ability.Cooldown;
            }
        }

        /// <summary>
        /// Sets the ability on the given index
        /// </summary>
        /// <param name="slot">Slot to set the ability</param>
        /// <param name="ability">The ability that is being added</param>
        public void SetAbility(uint slot, IAbility ability) => _abilities[slot] = ability;

        public IAbility GetAbility(int slot) => _abilities[slot];
        public bool IsAbilityOnCd(uint slot) => HasAbilityInSlot(slot) && _abilities[slot].CurrentCooldown > 0;
        public bool HasAbilityInSlot(uint slot) => _abilities.Length > slot && _abilities[slot] != null;
        #endregion
    }
}