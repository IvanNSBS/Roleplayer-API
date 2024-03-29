﻿using System;
using System.Collections.Generic;
using INUlib.Core.Math;
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

        /// <summary>
        /// During the cast state, when to unleash the spell.
        /// This value will be clamped between 0 and castTime so it will always fire at the start or at the end
        /// of the cast state.
        /// </summary>
        public readonly float unleashDuringCastTime;
        
        public readonly AbilityCastType castType;
        #endregion

        
        #region Methods
        [JsonConstructor]
        public TimelineData(float channelingTime, float overChannellingTime, float castTime, float recoveryTime, float unleashDuringCastTime, AbilityCastType castType)
        {
            this.channelingTime = channelingTime;
            this.overChannellingTime = overChannellingTime;
            this.castTime = castTime;
            this.recoveryTime = recoveryTime;
            this.unleashDuringCastTime = INUMath.Clamp(unleashDuringCastTime, 0, castTime);
            this.castType = castType;
        }
        #endregion
    }
    
    public class CastTimeline
    {
        #region Private Fields
        private TimelineData _data;
        private float _totalElapsedTime;
        private float _currentClbkElapsedTime;
        private bool _skipOverchanneling;
        
        private TimelineState _state;
        private CastingState _clbkState;
        private HashSet<Action> _eventsFired;

        private Dictionary<CastingState, Tuple<float, Action>> _clbkTimers;
        #endregion
        
        #region Properties
        public TimelineState state => _state;
        public CastingState clbkState => _clbkState;
        public TimelineData data => _data;
        public float TotalElapsedTime => _totalElapsedTime;
        public float CurrentStateElapsedTime => _currentClbkElapsedTime;
        #endregion
        
        #region Events
        public event Action TimelinePaused;
        public event Action TimelineStarted;
        public event Action Timeline_And_Recovery_Finished;

        public event Action ChannelingFinished_OverchannelingStarted = delegate {  };
        public event Action OverchannelingFinished_CastingStarted = delegate {  };
        public event Action CastFinished_ConcentrationStarted = delegate {  };
        public event Action UnleashAbility = delegate {  };
        public event Action ConcentrationFinished_RecoveryStarted = delegate {  };
        #endregion
        
        
        #region Methods
        public CastTimeline(TimelineData timelineData)
        {
            _data = timelineData;
            _eventsFired = new HashSet<Action>();

            SetupTimersCallback();
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
            if (_clbkState == CastingState.None)
                _clbkState = CastingState.Channeling;
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
            _totalElapsedTime = 0;
            _eventsFired.Clear();
            _state = TimelineState.Pending;
            _clbkState = CastingState.None;
        }

        public void FinishConcentration()
        {
            GoToNextState();
        }

        /// <summary>
        /// If set to true during Channeling, it will prepare to skip Overchanneling.
        /// If set to true during Overchanneling, it'll end it prematurely.
        /// Will have no effect if the casting state is not Channeling or Overchanneling
        /// </summary>
        /// <param name="skip">Whether or not to skip overchanneling</param>
        public void SkipOverchanneling(bool skip)
        {
            _skipOverchanneling = skip;
            if (_skipOverchanneling && _clbkState == CastingState.OverChanneling)
            {
                GoToNextState();                
            }
        }

        public void JumpToStartRecoveryState()
        {
            if (_clbkState == CastingState.CastRecovery)
                return;
            
            _clbkState = CastingState.Concentrating;
            _currentClbkElapsedTime = 0f;
            GoToNextState();
        }
        
        /// <summary>
        /// Updates the timeline, making time move forward
        /// </summary>
        /// <param name="deltaTime">How much time passed since the last frame</param>
        public void Update(float deltaTime)
        {
            if (_state != TimelineState.Running)
                return;
            
            _totalElapsedTime += deltaTime;
            _currentClbkElapsedTime += deltaTime;

            if (_clbkState == CastingState.Concentrating && data.castType == AbilityCastType.Concentration)
                return;

            if (CanUnleashDuringCasting())
            {
                UnleashAbility?.Invoke();
                _eventsFired.Add(UnleashAbility);
            }
            if (_currentClbkElapsedTime >= _clbkTimers[_clbkState].Item1)
            {
                GoToNextState();
                
                if (DidNotUnleashAfterIncreaseState())
                {
                    UnleashAbility?.Invoke();
                    _eventsFired.Add(UnleashAbility);
                }
            }
        }

        /// <summary>
        /// Sets the CastTimeline data.
        /// If set to null, will generate a timelineData with all times set to 0, meaning everything will be instant
        /// and keeping the old cast time. 
        /// </summary>
        /// <param name="newData"></param>
        public void UpdateTimelineData(TimelineData newData)
        {
            if (newData == null)
                _data = new TimelineData(0,0,0,0,0, _data.castType);
            else
                _data = newData;
            SetupTimersCallback();
        }

        /// <summary>
        /// Clear unleash callbacks. Necessary for the CastHandler to be able to
        /// recycle the unleash callback on subsequent cast requests.
        /// </summary>
        public void ClearUnleashCallbacks() => UnleashAbility = null;
        #endregion
        
        
        #region Helper Method
        protected void GoToNextState()
        {
            IncreaseStateAndFireCallbacks();
            if(_clbkState == CastingState.OverChanneling && _skipOverchanneling)
                IncreaseStateAndFireCallbacks();
            
            bool isAtConcentrating = _clbkState == CastingState.Concentrating;
            bool isConcentrationSpell = data.castType == AbilityCastType.Concentration;
            
            // Concentration step should be skipped if it's not a concentration ability
            if (isAtConcentrating && !isConcentrationSpell)
            {
                IncreaseStateAndFireCallbacks();
            }
            
            // Skip every state that has a timer very close to 0 so we don't need to wait for the next frame to fire it
            while((int)_clbkState <= (int)CastingState.CastRecovery && _clbkTimers[_clbkState].Item1 < 0.0001f)
            {
                isAtConcentrating = _clbkState == CastingState.Concentrating;
                if (isAtConcentrating && isConcentrationSpell)
                    break;
                
                IncreaseStateAndFireCallbacks();
            }
        }

        private void IncreaseStateAndFireCallbacks()
        {
            if (_clbkState > CastingState.CastRecovery)
                return;
            
            Action clbk = _clbkTimers[_clbkState].Item2;
            if (!_eventsFired.Contains(clbk))
            {
                clbk();
                _eventsFired.Add(clbk);
            }

            // dont set to 0. Elapsed time can go much beyond the time limit during a frame drop. If we simply
            // set _currentClbkElapsedTime to 0 like we were doing before, the timeline would surely "desync"
            // on frame spikes.
            _currentClbkElapsedTime -= _clbkTimers[_clbkState].Item1;
            _clbkState++;
            
            // Update with a deltaTime of 0. If the frame spike was big enough to trigger two state changes
            // we need to catch that.
            Update(0);
        }

        private bool CanUnleashDuringCasting()
        {
            bool isCasting = _clbkState == CastingState.Casting;
            bool isTimeToUnleash = _currentClbkElapsedTime >= _data.unleashDuringCastTime;
            bool notFiredAlready = !_eventsFired.Contains(UnleashAbility);

            return isCasting && isTimeToUnleash && notFiredAlready;
        }

        /// <summary>
        /// Can happen when cast times are all 0 and the unleash isn't caught during the update since casting
        /// will be skipped.
        /// </summary>
        private bool DidNotUnleashAfterIncreaseState()
        {
            return _clbkState > CastingState.Casting && !_eventsFired.Contains(UnleashAbility);
        }
        
        private void SetupTimersCallback()
        {
            _clbkTimers = new Dictionary<CastingState, Tuple<float, Action>>()
            {
                {
                    CastingState.Channeling,
                    new Tuple<float, Action>(_data.channelingTime, () => ChannelingFinished_OverchannelingStarted?.Invoke())
                },
                {
                    CastingState.OverChanneling,
                    new Tuple<float, Action>(_data.overChannellingTime, () => OverchannelingFinished_CastingStarted?.Invoke())
                },
                {
                    CastingState.Casting,
                    new Tuple<float, Action>(_data.castTime, () => CastFinished_ConcentrationStarted?.Invoke())
                },
                {
                    CastingState.Concentrating,
                    new Tuple<float, Action>(0, () => ConcentrationFinished_RecoveryStarted?.Invoke())
                },
                {
                    CastingState.CastRecovery,
                    new Tuple<float, Action>(_data.recoveryTime, () =>
                    {
                        Timeline_And_Recovery_Finished?.Invoke();
                        _state = TimelineState.Finished;
                    })
                },
            };
        }
        #endregion
    }
}