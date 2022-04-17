using UnityEngine;

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
        private TCaster _caster;

        private IAbilityObject _abilityObject;
        private CastHandlerPolicy _policy;
        private AbilitiesController<TAbility, TCaster> _controller;
        #endregion


        #region Properties
        public IAbilityObject AbilityObject => _abilityObject;
        #endregion


        #region Constructor
        public CastHandler(AbilitiesController<TAbility, TCaster> ctrl, TCaster caster, CastObjects castInfo)
        {
            _timesCastCalled = 0;
            _caster = caster;
            _controller = ctrl;

            _policy = castInfo.policy;
            _abilityObject = castInfo.abilityObject;

            OnCast();
        }
        #endregion


        #region Methods
        public void OnCast()
        {
            _timesCastCalled++;
            _policy?.OnCastRequested(_timesCastCalled, _controller.CastingState);
        }
        
        public void OnCastCanceled() => _policy?.OnStopCastRequested(_controller.CastingState);
        public void OnChannelingCompleted() => _policy?.OnChannelingCompleted();
        // public void OnCastCompleted() =>
        #endregion
    }
}
