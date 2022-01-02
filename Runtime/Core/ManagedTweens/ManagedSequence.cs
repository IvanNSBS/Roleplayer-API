
using System;
using DG.Tweening;
using UnityEngine;

namespace INUlib.Core.ManagedTweens
{
    public class ManagedSequence
    {
        #region Fields
        private Sequence _seq;
        private Transform _target;
        #endregion

        #region Properties
        public bool IsPlaying => _seq.IsActive();
        public Transform Target => _target;
        public bool ShouldResetPosition {get; set;}

        public TweenCallback OnPause
        {
            get => _seq.onPause;
            set => _seq.onPause = value;
        }

        public TweenCallback OnPlay 
        { 
            get => _seq.onPlay;
            set => _seq.onPlay = value;
        }

        public TweenCallback OnComplete 
        {
            get => _seq.onComplete;
            set => _seq.onComplete = value;
        }
        #endregion


        #region Constructor
        public ManagedSequence(Transform target)
        {
            _target = target;
            _seq = DOTween.Sequence();
            _seq.Restart();
            _seq.Pause();
        }

        public ManagedSequence(Transform target, Sequence seq)
        {
            _target = target;
            _seq = seq;
            _seq.Restart();
            _seq.Pause();
        }
        #endregion

        #region Methods
        public virtual void SetTarget(Transform tgt) => _target = tgt;
        protected virtual void Reset()
        {
            _target.transform.localScale = Vector3.one;
            _target.transform.rotation = Quaternion.identity;
            if(ShouldResetPosition)
                _target.transform.position = Vector3.zero;
        }

        public virtual void Play()
        {
            if(IsPlaying) {
                Reset();
                _seq.Restart();
            }
            else
                _seq.Play();
        }
        public virtual void Pause() => _seq.Pause();

        public void AppendCallback(TweenCallback clbk) => _seq.AppendCallback(clbk);
        public void InsertCallback(float at, TweenCallback clbk) => _seq.InsertCallback(at, clbk);
        public void AppendInterval(float interval) => _seq.AppendInterval(interval);
        public void Append(Tween tween) => _seq.Append(tween);
        public void Join(Tween tween) => _seq.Join(tween);
        public void Insert(float at, Tween tween) => _seq.Insert(at, tween);
        #endregion
    }
}