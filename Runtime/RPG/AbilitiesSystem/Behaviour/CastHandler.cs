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

        private Func<CastingState> _castStateGetter;
        private CastObjects _castObjects;
        #endregion


        #region Properties
        public AbilityObject AbilityObject => _castObjects.abilityObject;
        public int TimesCastCalled => _timesCastCalled;
        public CastTimeline Timeline => _castObjects.timeline;
        #endregion


        #region Constructor
        public CastHandler(CastObjects castObjects, Func<CastingState> castStateGetter)
        {
            _timesCastCalled = 0;
            _castStateGetter = castStateGetter;
            _castObjects = castObjects;
            
            SetupTimeline();
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
            _castObjects.policy?.OnCastRequested(_timesCastCalled, _castStateGetter());
        }
        
        /// <summary>
        /// Communicates to the CastPolicy that a cancel cast was requested
        /// </summary>
        public void OnCastCanceled() => _castObjects.policy?.OnCancelRequested(_castStateGetter());

        /// <summary>
        /// Updates the timeline and the ability object, passing the time that has passed since the last frame.
        /// It will try to finish concentration on every frame if the casting state is Casting and the ability
        /// is of concentration type by checking if ConcentrationEndCondition has been met.
        /// </summary>
        /// <param name="deltaTime">The amount of time that has passed since the last frame</param>
        public void Update(float deltaTime)
        {
            CastingState currentCastState = _castStateGetter();
            
            _castObjects.timeline?.Update(deltaTime);
            _castObjects.abilityObject.OnUpdate(deltaTime, currentCastState);
            if (currentCastState == CastingState.OverChanneling && _castObjects.timeline != null)
            {
                CastTimeline timeline = _castObjects.timeline;
                float elapsedOverchannel = timeline.ElapsedOverchannelingTime;
                float overchannelDuration = timeline.data.overChannellingTime;
                _castObjects.abilityObject.OnOverchannel(elapsedOverchannel, overchannelDuration);
            }
            
            if (currentCastState == CastingState.Casting && _castObjects.endConcentrationCondition())
            {
                _castObjects.timeline?.FinishConcentration();
            }
        }

        public void DrawGizmos()
        {
            _castObjects.abilityObject.OnDrawGizmos();
        }
        #endregion
        
        
        #region Helper Methods
        private void SetupTimeline()
        {
            _castObjects.timeline.CastFinished_RecoveryStarted += _castObjects.abilityObject.UnleashAbility;

            if (_castObjects.timeline.data.castType == AbilityCastType.FireAndForget)
            {
                _castObjects.timeline.Timeline_And_Recovery_Finished += _castObjects.abilityObject.InvokeNotifyFinishCast;
                _castObjects.timeline.Timeline_And_Recovery_Finished += _castObjects.abilityObject.InvokeNotifyDiscard;
            }
            
            _castObjects.timeline.Start();
        }
        #endregion
    }
}
