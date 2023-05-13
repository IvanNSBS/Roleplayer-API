using System;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Serves as a middle man to handle communication between the
    /// AbilityObject and the CastPolicy, so the user can
    /// customize how input will be handled for the AbilityObject
    /// </summary>
    public class CastHandler
    {
        #region Fields
        private int _timesCastCalled;

        private AbilityObject _abilityObject;
        private CastHandlerPolicy _policy;
        private Func<CastingState> _castStateGetter;
        private CastTimeline _castTimeline;
        #endregion


        #region Properties
        public AbilityObject AbilityObject => _abilityObject;
        public int TimesCastCalled => _timesCastCalled;
        #endregion


        #region Constructor
        public CastHandler(CastObjects castInfo, Func<CastingState> castStateGetter)
        {
            _timesCastCalled = 0;
            _castStateGetter = castStateGetter;
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
            _policy?.OnCastRequested(_timesCastCalled, _castStateGetter());
        }
        
        /// <summary>
        /// Communicates to the CastPolicy that a cancel cast was requested
        /// </summary>
        public void OnCastCanceled() => _policy?.OnCancelRequested(_castStateGetter());

        public void Update(float deltaTime)
        {
            _abilityObject.OnUpdate(deltaTime);
        }

        public void DrawGizmos()
        {
             _abilityObject.OnDrawGizmos();
        }
        #endregion
    }
}
