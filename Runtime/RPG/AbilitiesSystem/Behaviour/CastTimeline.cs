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
    public class CastTimeline
    {
        #region Serialized Fields
        public readonly float channelingTime;
        public readonly float recoveryTime;
        #endregion

        #region Private Fields
        private float _elapsedTime;
        private float _timelineDuration;
        private TimelineState _state;
        private HashSet<Action> _eventsFired;
        #endregion

        #region Properties
        public TimelineState state => _state;
        public float ElapsedTime => _elapsedTime;
        public float CompletePercent => _elapsedTime / _timelineDuration; 
        #endregion
        
        #region Events
        public event Action TimelineFinished;
        public event Action TimelinePaused;

        public event Action ChannelingStarted;
        public event Action ChannelingFinished;
        public event Action RecoveryStarted;
        public event Action RecoveryFinished;
        #endregion
        
        
        #region Methods
        [JsonConstructor]
        public CastTimeline(float channelingTime, float recoveryTime)
        {
            this.channelingTime = channelingTime;
            this.recoveryTime = channelingTime + recoveryTime;

            _timelineDuration = channelingTime + recoveryTime;
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
            
            ChannelingStarted?.Invoke();
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
        
        public void Update(float deltaTime)
        {
            if (_state != TimelineState.Running)
                return;
            
            _elapsedTime += deltaTime;
            if (_elapsedTime >= channelingTime && !_eventsFired.Contains(ChannelingFinished))
            {
                ChannelingFinished?.Invoke();
                RecoveryStarted?.Invoke();
                _eventsFired.Add(ChannelingFinished);
            }
            if(_elapsedTime >= recoveryTime && !_eventsFired.Contains(RecoveryFinished))
            {
                RecoveryFinished?.Invoke();
                _eventsFired.Add(RecoveryFinished);
            }

            if (_elapsedTime >= _timelineDuration)
            {
                TimelineFinished?.Invoke();
                _state = TimelineState.Finished;
            }
        }
        #endregion
    }
}