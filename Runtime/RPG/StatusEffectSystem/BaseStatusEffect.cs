namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Base StatusEffect class.
    /// Manages all the necessary functionality for all types of StatusEffects, such as ActiveTime
    /// how much time it'll last and functions to complete it, as well as giving a better API for creating
    /// new Status Effects
    /// </summary>
    /// <typeparam name="T">
    /// A CRTP for the StatusEffect, to give better readability and usability for the Reapply API
    /// </typeparam>
    public abstract class BaseStatusEffect<T> : IStatusEffect where T : BaseStatusEffect<T>
    {
        #region Fields
        /// <summary>
        /// Wheter or not the effect was simply completed
        /// </summary>
        protected bool _completed;
        
        /// <summary>
        /// Whether or not the effect has been already applied to a StatusEffectManager
        /// </summary>
        protected bool _applied = false;

        /// <summary>
        /// For how much time, in seconds, the effect has been active since it was 
        /// applied to a StatusEffectManager
        /// </summary>
        protected float _activeTime;

        /// <summary>
        /// How much time, in seconds, the StatusEffect is supposed to last.
        /// A value lower than 0 means the effect will last forever, until the Complete function is called.
        /// A value lower than 0 should be used for StatusEffects that should be removed only through events
        /// </summary>
        protected float _duration;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a StatusEffect that will last for a given duration, in seconds
        /// </summary>
        /// <param name="duration">How much time the StatusEffect should last</param>
        public BaseStatusEffect(float duration)
        {
            _activeTime = 0;
            _duration = duration;
        }

        /// <summary>
        /// Creates a status effect that will last until the Complete function is called
        /// </summary>
        public BaseStatusEffect()
        {
            _activeTime = 0;
            _duration = -1;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Applies the StatusEffect. The ApplyStats are passed for things such as Diminishing Returns
        /// </summary>
        /// <param name="stats">The EffectApplyStats for this StatusEffect</param>
        public void Apply(EffectApplyStats stats)
        {
            _applied = true;
            OnApply(stats);
        }

        /// <summary>
        /// Completes the StatusEffect, marking it to be removed to the StatusEffectManager in the next frame
        /// </summary>
        public void Complete() => _completed = true;

        /// <summary>
        /// Reapplies this StatusEffect
        /// </summary>
        /// <param name="ef">
        /// The StatusEffect that was trying to be applied. Will be cast to the T generic 
        /// that is supposed to be a CRTP
        /// </param>
        /// <param name="stats">The EffectApplyStats for this StatusEffect</param>
        public void Reapply(IStatusEffect ef, EffectApplyStats stats) => OnReapply((T)ef, stats);

        /// <summary>
        /// Updates the StatusEffect, updating the ActiveTime.
        /// </summary>
        /// <param name="deltaTime">
        /// The time since the last frame
        /// </param>
        /// <returns>
        /// True if the effect has been completed, meaning the Complete function was called or
        /// the ActiveTime is greater than the duration, if the status effect does not last for a undefined
        /// amount of time.
        /// False if the effect has not been completed
        /// </returns>
        public virtual bool Update(float deltaTime)
        {
            if(!_applied)
                return false;
            
            _activeTime += deltaTime;

            if(_duration < 0f)
                return _completed;
            else
                return _activeTime >= _duration || _completed; 
        }

        /// <summary>
        /// Actual reapply behaviour for the StatusEffect
        /// </summary>
        /// <param name="effect">The object that is trying to be added</param>
        /// <param name="stats">The EffectApplyStats for this StatusEffect</param>
        protected virtual void OnReapply(T effect, EffectApplyStats stats)
        {
            _activeTime = 0f;
        }

        /// <summary>
        /// Actual Apply behaviour for a status effect
        /// </summary>
        /// <param name="stats">The EffectApplyStats for this StatusEffect</param>
        public abstract void OnApply(EffectApplyStats stats);

        /// <summary>
        /// Function to be called once the StatusEffect has been completed
        /// </summary>
        public abstract void OnComplete();

        /// <summary>
        /// Function to be called if the StatusEffect has been dispeled(removed) before completing
        /// </summary>
        public abstract void OnDispel();
        #endregion
    }
}