using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace INUlib.RPG.AbilitiesSystem
{
    public enum TimelineState
    {
        Pending = 0,
        Running = 1,
        Paused = 2,
        Finished = 3
    }

    [Serializable]
    public class TimelineData
    {
        public readonly float channelingTime;
        public readonly float castTime;
        public readonly float recoveryTime;
        public readonly AbilityCastType castType;
        
        [JsonConstructor]
        public TimelineData(float channelingTime, float castTime, float recoveryTime, AbilityCastType castType)
        {
            this.channelingTime = channelingTime;
            this.castTime = castTime + channelingTime;
            this.recoveryTime = channelingTime + recoveryTime + castTime;
            this.castType = castType;
        }
    }
    
    public class CastTimeline
    {
        #region Private Fields
        private TimelineData _data;
        private float _elapsedTime;
        private TimelineState _state;
        private HashSet<Action> _eventsFired;
        #endregion

        #region Properties
        public TimelineState state => _state;
        public float ElapsedTime => _elapsedTime;
        public float CompletePercent => _elapsedTime / _data.recoveryTime; 
        #endregion
        
        #region Events
        public event Action TimelinePaused;
        public event Action TimelineStarted;
        public event Action Timeline_And_Recovery_Finished;

        public event Action ChannelingFinished_CastStarted = delegate {  };
        public event Action CastFinished_RecoveryStarted = delegate {  };
        #endregion
        
        
        #region Methods
        public CastTimeline(TimelineData timelineData)
        {
            _data = timelineData;
            _eventsFired = new HashSet<Action>();
        }

        /// <summary>
        /// Starts the timeline, allowing it to receive updates
        /// If the timeline has finished, it'll be restarted as well
        /// </summary>
        public void Start()
        {
            if(_state == TimelineState.Finished)
                Reset();
            
            TimelineStarted?.Invoke();
            _state = TimelineState.Running;
        }

        /// <summary>
        /// Pauses the timeline
        /// </summary>
        public void Pause()
        {
            TimelinePaused?.Invoke();
            _state = TimelineState.Paused;
        }

        /// <summary>
        /// Resets the timeline, allowing it to run again
        /// </summary>
        public void Reset()
        {
            _elapsedTime = 0;
            _eventsFired.Clear();
            _state = TimelineState.Pending;
        }
        
        /// <summary>
        /// Finishes the timeline. Manual calls have no effect if cast type is Fire and Forget.
        /// It is called automatically at the end of the timeline if cast type is Fire and Forget.
        /// Tries to finish the timeline if the cast time has passed and cast type is Concentration.
        /// Has no effect if cast type is Concentration but the CastFinished and Recovery Started haven't
        /// been called yet
        /// </summary>
        /// <returns>True if the timeline has been finished by this call. False otherwise</returns>
        public bool FinishTimeline()
        {
            bool concentrationCanFinish = _eventsFired.Contains(CastFinished_RecoveryStarted) &&
                                          _data.castType == AbilityCastType.Concentration;

            bool fireAndForgetCanFinish = _eventsFired.Contains(CastFinished_RecoveryStarted);
            bool notFired = !_eventsFired.Contains(Timeline_And_Recovery_Finished);
            if (notFired && (fireAndForgetCanFinish || concentrationCanFinish))
            {
                Timeline_And_Recovery_Finished?.Invoke();
                _eventsFired.Add(Timeline_And_Recovery_Finished);
                _state = TimelineState.Finished;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Updates the timeline, making time move forward
        /// </summary>
        /// <param name="deltaTime">How much time passed since the last frame</param>
        public void Update(float deltaTime)
        {
            if (_state != TimelineState.Running)
                return;
            
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _data.channelingTime && !_eventsFired.Contains(ChannelingFinished_CastStarted))
            {
                ChannelingFinished_CastStarted?.Invoke();
                _eventsFired.Add(ChannelingFinished_CastStarted);
            }
            if (_elapsedTime >= _data.castTime && !_eventsFired.Contains(CastFinished_RecoveryStarted))
            {
                CastFinished_RecoveryStarted?.Invoke();
                _eventsFired.Add(CastFinished_RecoveryStarted);
            }

            if(
                _data.castType == AbilityCastType.FireAndForget && 
                _elapsedTime >= _data.recoveryTime && 
                !_eventsFired.Contains(Timeline_And_Recovery_Finished)
            )
            {
                FinishTimeline();
            }
        }
        #endregion
    }
}