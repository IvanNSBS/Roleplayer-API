namespace INUlib.RPG.StatusEffectSystem
{
    public abstract class BaseStatusEffect<T> : IStatusEffect where T : BaseStatusEffect<T>
    {
        #region Fields
        private bool _applied = false;
        private float _activeTime;
        private float _duration;
        #endregion


        #region Constructors
        public BaseStatusEffect(float duration)
        {
            _activeTime = 0;
            _duration = duration;
        }

        public BaseStatusEffect()
        {

        }
        #endregion


        #region Methods
        public void Apply()
        {
            _applied = true;
            OnApply();
        }

        public void Reapply(IStatusEffect ef) => OnReapply((T)ef);

        public abstract void OnApply();
        public abstract void OnComplete();
        public abstract void OnDispel();
        protected abstract void OnReapply(T effect);
        public virtual bool Update(float deltaTime)
        {
            if(!_applied)
                return false;
                
            _activeTime += deltaTime;
            return _activeTime >= deltaTime; 
        }
        #endregion
    }
}