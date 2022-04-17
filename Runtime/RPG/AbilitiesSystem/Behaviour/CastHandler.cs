using UnityEngine.AI;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Default controller for agents that can use the ability system
    /// Fully manages the abilities cooldown and their casting process
    /// </summary>
    public class CastHandler<TAbility, TCaster> 
           where TAbility : class, IAbility<TCaster> where TCaster : IAbilityCaster
    {
        #region Fields
        private int _timesCastCalled;
        private TAbility _ability;
        private TCaster _caster;

        private IAbilityObject<TCaster> _abilityObject;
        private CastHandlerPolicy<IAbilityObject<TCaster>, TCaster> _policy;
        private AbilitiesController<TAbility, TCaster> _controller;
        #endregion


        #region Properties
        #endregion


        #region Constructor
        public CastHandler(AbilitiesController<TAbility, TCaster> ctrl, TAbility ability, TCaster caster)
        {
            _timesCastCalled = 0;
            _ability = ability;
            _caster = caster;
            _controller = ctrl;

            // _abilityObject = _ability.CreateObject();
            // _policy = _ability.GetCastPolicy();

            OnCast();
        }
        #endregion


        #region Methods
        public void OnDrawGizmos() => _abilityObject.OnDrawGizmos();

        public void OnCast()
        {
            _timesCastCalled++;
            _policy.OnCastRequested(_timesCastCalled, _controller.CastingState);
        }
        
        public void OnCastCanceled() => _policy.OnStopCastRequested(_controller.CastingState);
        public void OnChannelingCompleted() => _policy.OnChannelingCompleted();

        #endregion
    }
}
