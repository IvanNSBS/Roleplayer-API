
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Core.ManagedTweens
{
    /// <summary>
    /// Managed sequences are complete Animations made with
    /// DoTween sequences that needs to be played over and 
    /// over again, and thus needs to be managed to make sure
    /// the sequence can be played repeatedly, sometimes even 
    /// before finishing, and the animation won't break .
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
        public ManagedSequence()
        {
            _targets = new List<IManagedTarget>();
            InitializeSequence();
        }
        
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
        /// <summary>
        /// Adds a IManagedTarget to the target list 
        /// </summary>
        /// <param name="tgt">The target to be added</param>
        public virtual void AddTarget(IManagedTarget tgt) => _targets.Add(tgt);
        
        /// <summary>
        /// Adds a transform to the target list, creating a ManagedTransform with
        /// the default constructor for it
        /// </summary>
        /// <param name="tgt">The transform to create the ManagedTranform</param>
        public virtual void AddTarget(Transform tgt) => _targets.Add(new ManagedTransform(tgt));
        
        /// <summary>
        /// Appends a list of IManagedTarget to the target list
        /// </summary>
        /// <param name="tgts">The list of IManagedTargets to insert to the target list</param>
        public virtual void AddTarget(List<IManagedTarget> tgts) => _targets.AddRange(tgts);

        /// <summary>
        /// Removes a target from the target list
        /// </summary>
        /// <param name="tgt">The target to remmove</param>
        public virtual void RemoveTarget(IManagedTarget tgt) => _targets.Remove(tgt);

        /// <summary>
        /// Resets
        /// </summary>
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

        /// <summary>
        /// Appends a callback to the end of the managed sequence
        /// </summary>
        /// <param name="clbk">The callback to be appended</param>
        public void AppendCallback(TweenCallback clbk) => _seq.AppendCallback(clbk);

        /// <summary>
        /// Inserts a callback to the managed sequence at the given time in the timeline
        /// </summary>
        /// <param name="at">The position to insert the callback</param>
        /// <param name="clbk">The callback to be inserted</param>
        public void InsertCallback(float at, TweenCallback clbk) => _seq.InsertCallback(at, clbk);

        /// <summary>
        /// Appends a interval to the end of the managed sequence
        /// </summary>
        /// <param name="interval">How much time to wait</param>
        public void AppendInterval(float interval) => _seq.AppendInterval(interval);

        /// <summary>
        /// Appends a tween to the end of the managed sequence
        /// </summary>
        /// <param name="tween">The tween to be added</param>
        public void Append(Tween tween) => _seq.Append(tween);

        /// <summary>
        /// Inserts a tween at the same time as the position in the timeline
        /// as the last tween, callback or interval added to the managed sequence.
        /// Note that, in case of a Join after an interval, 
        /// the insertion time will be the time where the interval starts, 
        /// not where it finishes.
        /// </summary>
        /// <param name="tween"></param>
        public void Join(Tween tween) => _seq.Join(tween);

        /// <summary>
        /// Inserts a tween to the managed sequence at the given time in the timeline
        /// </summary>
        /// <param name="at">The position to insert the tween</param>
        /// <param name="tween">The tween to be inserted</param>
        public void Insert(float at, Tween tween) => _seq.Insert(at, tween);

        /// <summary>
        /// Implicit operator to cast ManagedSequence to the DoTween.Sequence
        /// it manages. Useful for nesting ManagedSequences when creating custom
        /// ones
        /// </summary>
        /// <param name="ms">The target ManagedSequence</param>
        public static implicit operator Sequence(ManagedSequence ms) => ms._seq;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to initialize the DoTween sequence
        /// </summary>
        protected void InitializeSequence()
        {
            _seq = DOTween.Sequence();
            _seq.SetAutoKill(false);
            _seq.Restart();
            _seq.Pause();
        }

        /// <summary>
        /// Helper method to initialize the DoTween sequence
        /// Makes the ManagedSequence be the same as the given Sequence
        /// </summary>
        /// <param name="seq"></param>
        protected void InitializeSequence(Sequence seq)
        {
            _seq = seq;
            _seq.SetAutoKill(false);
            _seq.Restart();
            _seq.Pause();
        }
        #endregion
    }
}