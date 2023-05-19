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
        #region Fields
        /// <summary>
        /// Channeling time represents the time the ability needs to
        /// be charged to be able to start
        /// </summary>
        public readonly float channelingTime;

        /// <summary>
        /// Overchannelling time represents how much time after finishing channeling
        /// an ability might keep on channeling to overcharge the effect 
        /// </summary>
        public readonly float overChannellingTime;
        
        /// <summary>
        /// Cast Time represents the time after channeling where the ability is being
        /// unleashed into the world. It can be 0 for an instant cast  
        /// </summary>
        public readonly float castTime;
        
        /// <summary>
        /// Recovery Time represents the time after the ability was casted that an actor needs
        /// to wait to be able to try to cast another ability again
        /// </summary>
        public readonly float recoveryTime;
        
        public readonly AbilityCastType castType;
        #endregion

        
        #region Methods
        [JsonConstructor]
        public TimelineData(float channelingTime, float overChannellingTime, float castTime, float recoveryTime, AbilityCastType castType)
        {
            this.channelingTime = channelingTime;
            this.overChannellingTime = overChannellingTime;
            this.castTime = castTime;
            this.recoveryTime = recoveryTime;
            this.castType = castType;
        }
        #endregion
    }
    
    public class CastTimeline
    {
        #region Private Fields
        private TimelineData _data;
        private float _elapsedTime;
        private TimelineState _state;
        private HashSet<Action> _eventsFired;
        private bool _concentrating;

        private float _concentrationRecoveryTime = 0f;
        #endregion
        
        #region Properties
        protected float ChannelingFinishTime => _data.channelingTime;
        protected float OverchannellingFinishTime => ChannelingFinishTime + _data.overChannellingTime;
        protected float CastFinishTime => OverchannellingFinishTime + _data.castTime;
        protected float RecoveryFinishTime => CastFinishTime + _data.recoveryTime;
        
        public TimelineState state => _state;
        public TimelineData data => _data;
        public float ElapsedTime => _elapsedTime;
        public float CompletePercent => _elapsedTime / _data.recoveryTime;

        public float ElapsedOverchannelingTime
        {
            get
            {
                if (data.overChannellingTime < 0.001f || ElapsedTime > OverchannellingFinishTime || ElapsedTime < ChannelingFinishTime)
                    return -1;

                float time = ElapsedTime - ChannelingFinishTime;
                return time;
            }
        }
        #endregion
        
        #region Events
        public event Action TimelinePaused;
        public event Action TimelineStarted;
        public event Action Timeline_And_Recovery_Finished;

        public event Action ChannelingFinished_OverchannelingStarted = delegate {  };
        public event Action OverchannelingFinished_CastingStarted = delegate {  };
        public event Action CastFinished_RecoveryStarted = delegate {  };
        #endregion
        
        
        #region Methods
        public CastTimeline(TimelineData timelineData)
        {
            _data = timelineData;
            _eventsFired = new HashSet<Action>();
            _concentrating = data.castType == AbilityCastType.Concentration;
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

        public void FinishConcentration()
        {
            _concentrating = false;
            // need to subtract from cast time to get actual recovery time because i'm doing weird math
            // since there's no real timeline(yet) and things are quite manual
            _concentrationRecoveryTime = _data.recoveryTime + _elapsedTime;
            Update(0);            
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
            if (_elapsedTime >= ChannelingFinishTime && !_eventsFired.Contains(ChannelingFinished_OverchannelingStarted))
            {
                ChannelingFinished_OverchannelingStarted?.Invoke();
                _eventsFired.Add(ChannelingFinished_OverchannelingStarted);
            }

            if (_elapsedTime >= OverchannellingFinishTime &&
                !_eventsFired.Contains(OverchannelingFinished_CastingStarted))
            {
                OverchannelingFinished_CastingStarted?.Invoke();
                _eventsFired.Add(OverchannelingFinished_CastingStarted);
            }

            if (!_concentrating)
            {
                float castFinish = _data.castType == AbilityCastType.Concentration ? 0 : CastFinishTime;
                if (_elapsedTime >= castFinish && !_eventsFired.Contains(CastFinished_RecoveryStarted))
                {
                    CastFinished_RecoveryStarted?.Invoke();
                    _eventsFired.Add(CastFinished_RecoveryStarted);
                }

                float recoveryFinish = _data.castType == AbilityCastType.Concentration
                    ? _concentrationRecoveryTime
                    : RecoveryFinishTime;
                
                if(_elapsedTime >= recoveryFinish && !_eventsFired.Contains(Timeline_And_Recovery_Finished))
                {
                    FinishTimeline();
                }
            }
        }
        #endregion
        
        
        #region Helper Method
        /// <summary>
        /// Finishes the timeline. Manual calls have no effect if cast type is Fire and Forget.
        /// It is called automatically at the end of the timeline if cast type is Fire and Forget.
        /// Tries to finish the timeline if the cast time has passed and cast type is Concentration.
        /// Has no effect if cast type is Concentration but the CastFinished and Recovery Started haven't
        /// been called yet
        /// </summary>
        /// <returns>True if the timeline has been finished by this call. False otherwise</returns>
        protected bool FinishTimeline()
        {
            bool notFired = !_eventsFired.Contains(Timeline_And_Recovery_Finished);
            if (notFired)
            {
                Timeline_And_Recovery_Finished?.Invoke();
                _eventsFired.Add(Timeline_And_Recovery_Finished);
                _state = TimelineState.Finished;
                return true;
            }

            return false;
        }
        #endregion
    }
}