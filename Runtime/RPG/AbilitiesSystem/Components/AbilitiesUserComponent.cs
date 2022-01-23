using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Humble Component for the Abilities Controller class 
    /// to be used on the Unity Environment 
    /// </summary>
    public abstract class AbilitiesUserComponent<TAbility, TAbilityDataFactory> : MonoBehaviour where TAbility: class, 
    IAbility<TAbilityDataFactory> where TAbilityDataFactory: IAbilityDataHub
    {
        #region Fields
        protected AbilitiesController<TAbility, TAbilityDataFactory> _controller;
        #endregion


        #region MonoBehaviour Methods
        protected virtual void Update()
        {
            _controller?.Update(Time.deltaTime);
        }
        #endregion


        #region Methods
        public virtual void Initialize(uint slotAmount, TAbilityDataFactory dataHub) => 
            _controller = new AbilitiesController<TAbility, TAbilityDataFactory>(slotAmount, dataHub);
        public virtual void StartChanneling(uint slot) => _controller.StartChanneling(slot);
        public virtual TAbility GetAbilityBeingCast() => _controller.GetCastingAbility();
        public virtual void CancelCast() => _controller.CancelChanneling();
        public virtual float GetElapsedCastingTime() => _controller.ElapsedCastingTime;
        public virtual void SetAbility(uint slot, TAbility ability) => _controller.SetAbility(slot, ability);
        public virtual TAbility GetAbility(uint slot) => _controller.GetAbility(slot);
        public virtual bool IsAbilityOnCD(uint slot) => _controller.IsAbilityOnCd(slot);
        public virtual bool HasAbilityInSlot(uint slot) => _controller.HasAbilityInSlot(slot);
        #endregion
    }
}