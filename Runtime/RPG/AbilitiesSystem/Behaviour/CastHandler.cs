namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Communicates cast input and state to the CastPolicy, so the user can
    /// customize how input will be handled for the AbilityObject
    /// </summary>
    public class CastHandler<TAbility, TCaster> 
           where TAbility : class, IAbility<TCaster> where TCaster : ICasterInfo
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
        public int TimesCastCalled => _timesCastCalled;
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
        /// <summary>
        /// Receives a cast request, incrementing how many times the cast was called
        /// and invoking the OnCastRequested for the CastHandlerPolicy
        /// </summary>
        public void OnCast()
        {
            _timesCastCalled++;
            _policy?.OnCastRequested(_timesCastCalled, _controller.CastingState);
        }
        
        /// <summary>
        /// Communicates to the CastPolicy that a cancel cast was requested
        /// </summary>
        public void OnCastCanceled() => _policy?.OnCancelRequested(_controller.CastingState);
        #endregion
    }
}
