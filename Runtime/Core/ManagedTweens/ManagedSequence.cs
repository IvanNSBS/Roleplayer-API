
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Core.ManagedTweens
{
    /// <summary>
    /// Managed sequences are DoTween sequences that needs to be played
    /// over and over again, and thus needs to be managed to make sure
    /// the sequence can be played repeatedly, sometimes even before 
    /// finishing, and the animation won't break 
    /// </summary>
    public class ManagedSequence
    {
        #region Fields
        private Sequence _seq;
        private List<IManagedTarget> _targets;
        #endregion

        #region Properties
        public bool IsPlaying => _seq.IsPlaying();
        public IReadOnlyList<IManagedTarget> Targets => _targets;

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


        #region Constructors
        public ManagedSequence(Transform target)
        {
            _targets = new List<IManagedTarget>();
            _targets.Add(new ManagedTransform(target));

            InitializeSequence();
        }
        
        public ManagedSequence(Transform target, Sequence seq)
        {
            _targets = new List<IManagedTarget>();
            _targets.Add(new ManagedTransform(target));

            InitializeSequence(seq);
        }

        public ManagedSequence(Transform[] targets)
        {
            _targets = new List<IManagedTarget>();
            foreach(var tgt in targets)
                _targets.Add(new ManagedTransform(tgt));

            InitializeSequence();
        }

        public ManagedSequence(IManagedTarget target)
        {
            _targets = new List<IManagedTarget>();
            _targets.Add(target);

            InitializeSequence();
        }

        public ManagedSequence(IManagedTarget[] targets)
        {
            _targets = new List<IManagedTarget>();
            _targets.AddRange(targets);

            InitializeSequence();
        }

        ~ManagedSequence() => _seq = null;
        #endregion


        #region Methods
        public virtual void AddTarget(IManagedTarget tgt) => _targets.Add(tgt);
        public virtual void AddTarget(List<IManagedTarget> tgts) => _targets.AddRange(tgts);
        public virtual void RemoveTarget(IManagedTarget tgt) => _targets.Remove(tgt);
        protected virtual void Reset()
        {
            foreach(var tgt in _targets)
                tgt.Reset();
        }

        /// <summary>
        /// Plays the sequence, resetting and restarting if necessary
        /// to make sure the animation won't break
        /// </summary>
        public virtual void Play()
        {
            Reset();
            _seq.Restart();

            if(!IsPlaying)
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


        #region Helper Methods
        private void InitializeSequence()
        {
            _seq = DOTween.Sequence();
            _seq.SetAutoKill(false);
            _seq.Restart();
            _seq.Pause();
        }

        private void InitializeSequence(Sequence seq)
        {
            _seq = seq;
            _seq.SetAutoKill(false);
            _seq.Restart();
            _seq.Pause();
        }
        #endregion
    }
}