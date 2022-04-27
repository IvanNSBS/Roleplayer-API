using UnityEngine;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Humble Component for the Abilities Controller class 
    /// to be used on the Unity Environment 
    /// </summary>
    public abstract class AbilitiesUserComponent<TAbility, TCaster> : MonoBehaviour where TAbility: class, 
    IAbility<TCaster> where TCaster: ICasterInfo
    {
        #region Fields
        protected AbilitiesController<TAbility, TCaster> _controller;
        #endregion


        #region MonoBehaviour Methods
        protected virtual void Update()
        {
            _controller?.Update(Time.deltaTime);
        }

        protected virtual void OnDrawGizmos() => _controller?.OnDrawGizmos();
        #endregion


        #region Methods
        public CooldownHandler GetCooldownHandler() => _controller.CooldownsHandler;
        public virtual void Initialize(uint slotAmount, TCaster dataHub) => 
            _controller = new AbilitiesController<TAbility, TCaster>(slotAmount, dataHub);
        public virtual void StartChanneling(uint slot) => _controller.StartChanneling(slot);
        public virtual TAbility GetAbilityBeingCast() => _controller.GetCastingAbility();
        public virtual void CancelCast() => _controller.CancelChanneling();
        public virtual float GetElapsedChannelingTime() => _controller.ElapsedChannelingTime;
        public virtual void SetAbility(uint slot, TAbility ability) => _controller.SetAbility(slot, ability);
        public virtual TAbility GetAbility(uint slot) => _controller.GetAbility(slot);
        public virtual bool IsAbilityOnCD(uint slot) => _controller.CooldownsHandler.IsAbilityOnCd(slot);
        public virtual bool HasAbilityInSlot(uint slot) => _controller.HasAbilityInSlot(slot);
        #endregion
    }
}