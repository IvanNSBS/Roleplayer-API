namespace INUlib.RPG.StatusEffectSystem
{
    public abstract class BaseStatusEffect<T> : IStatusEffect where T : BaseStatusEffect<T>
    {
        #region Fields
        protected bool _completed;
        protected bool _applied = false;
        protected float _activeTime;
        protected float _duration;
        #endregion


        #region Constructors
        public BaseStatusEffect(float duration)
        {
            _activeTime = 0;
            _duration = duration;
        }

        public BaseStatusEffect()
        {
            _activeTime = 0;
            _duration = -1;
        }
        #endregion


        #region Methods
        public void Apply()
        {
            _applied = true;
            OnApply();
        }

        public void Complete() => _completed = true;
        public void Reapply(IStatusEffect ef) => OnReapply((T)ef);
       
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

        protected virtual void OnReapply(T effect)
        {
            _activeTime = 0f;
        }


        public abstract void OnApply();
        public abstract void OnComplete();
        public abstract void OnDispel();
        #endregion
    }
}