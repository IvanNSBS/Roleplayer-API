using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Humble Component for the Abilities Controller class 
    /// to be used on the Unity Environment 
    /// </summary>
    class AbilitiesUser : MonoBehaviour
    {
        #region Inspector Fields
        #endregion

        #region Fields
        private AbilitiesController _controller;
        #endregion


        #region MonoBehaviour Methods
        private void Update()
        {
            _controller?.Update(Time.deltaTime);
        }
        #endregion


        #region Methods
        public void Initialize(uint slotAmount) => _controller = new AbilitiesController(slotAmount);
        public void StartCasting(uint slot) => _controller.StartCast(slot);
        public IAbility GetAbilityBeingCast() => _controller.GetCastingAbility();
        public void CancelCast() => _controller.CancelCast();
        public float GetElapsedCastingTime() => _controller.ElapsedCastingTime;
        public void SetAbility(uint slot, IAbility ability) => _controller.SetAbility(slot, ability);
        public IAbility GetAbility(uint slot) => _controller.GetAbility(slot);
        public bool IsAbilityOnCD(uint slot) => _controller.IsAbilityOnCd(slot);
        public bool HasAbilityInSlot(uint slot) => _controller.HasAbilityInSlot(slot);
        #endregion
    }
}