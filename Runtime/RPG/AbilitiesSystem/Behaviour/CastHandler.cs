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
        private int _timesRecastCalled;

        private Func<CastingState> _castStateGetter;
        private CastObjects _castObjects;
        private IAbilityBase _casting;
        #endregion


        #region Properties
        public AbilityBehaviour AbilityBehaviour => _castObjects.AbilityBehaviour;
        public int TimesRecastCalled => _timesRecastCalled;
        public CastTimeline Timeline => _castObjects.timeline;
        public IAbilityBase Parent => _casting;
        #endregion


        #region Constructor
        public CastHandler(IAbilityBase casting, CastObjects castObjects, Func<CastingState> castStateGetter)
        {
            _casting = casting;
            _timesRecastCalled = 0;
            _castStateGetter = castStateGetter;
            _castObjects = castObjects;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Receives a cast request, incrementing how many times the cast was called
        /// and invoking the OnCastRequested for the CastHandlerPolicy
        /// </summary>
        public void OnAnotherCastRequested()
        {
            _timesRecastCalled++;
            TimelineData recastTimelineData;
            if (_casting.OnRecastTimelines == null || _casting.OnRecastTimelines.Length == 0 ||
                _timesRecastCalled >= _casting.OnRecastTimelines.Length)
            {
                recastTimelineData = null;
            }
            else
            {
                recastTimelineData = _casting.OnRecastTimelines[_timesRecastCalled];
            }
            
            _castObjects.timeline.Reset();
            _castObjects.timeline.ClearUnleashCallbacks();
            _castObjects.timeline.UpdateTimelineData(recastTimelineData);
            _castObjects.timeline.UnleashAbility += () => _castObjects.AbilityBehaviour.OnNewCastRequested(_timesRecastCalled, _castStateGetter());
            
            _castObjects.timeline.Start();
        }
        
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
            _castObjects.AbilityBehaviour.OnUpdate(deltaTime, currentCastState);
            
            if (currentCastState == CastingState.OverChanneling && _castObjects.timeline != null)
            {
                CastTimeline timeline = _castObjects.timeline;
                float elapsedOverchannel = timeline.CurrentStateElapsedTime;
                float overchannelDuration = timeline.data.overChannellingTime;
                _castObjects.AbilityBehaviour.OnOverchannel(elapsedOverchannel, overchannelDuration);
            }
            
            if (_castObjects.endConcentrationCondition != null && currentCastState == CastingState.Concentrating && _castObjects.endConcentrationCondition())
            {
                _castObjects.timeline?.FinishConcentration();
            }
        }

        public void DrawGizmos()
        {
            _castObjects.AbilityBehaviour.OnDrawGizmos();
        }
        #endregion
        
        
        #region Helper Methods
        public void SetupAbilityBehaviourTimelineCallbacks()
        {
            _castObjects.timeline.UnleashAbility += _castObjects.AbilityBehaviour.OnAbilityUnleashed;

            if (_casting.DiscardPolicy == DiscardPolicy.Auto)
            {
                _castObjects.timeline.Timeline_And_Recovery_Finished += _castObjects.AbilityBehaviour.InvokeNotifyDiscard;
            }
            
            _castObjects.timeline.ChannelingFinished_OverchannelingStarted += _castObjects.AbilityBehaviour.OnChannelingFinishedAndOverchannelingStarted;
            _castObjects.timeline.OverchannelingFinished_CastingStarted += _castObjects.AbilityBehaviour.OnOverChannelingFinishedAndCastStarted;
            _castObjects.timeline.CastFinished_ConcentrationStarted += _castObjects.AbilityBehaviour.OnCastFinishedConcentrationStartedAndConcentrationStarted;
            _castObjects.timeline.ConcentrationFinished_RecoveryStarted += _castObjects.AbilityBehaviour.OnConcentrationFinishedAndRecoveryStarted;
            
            _castObjects.timeline.Timeline_And_Recovery_Finished += _castObjects.AbilityBehaviour.OnRecoveryFinished;

            _castObjects.timeline.Start();
        }
        #endregion
    }
}
