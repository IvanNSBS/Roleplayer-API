namespace INUlib.RPG.StatusEffectSystem
{
    public abstract class StatusEffect
    {
        #region Fields
        private bool _applied = false;
        private float _activeTime;
        private float _duration;
        #endregion


        #region Constructors
        public StatusEffect(float duration)
        {
            _activeTime = 0;
            _duration = duration;
        }

        public StatusEffect()
        {

        }
        #endregion


        #region Methods
        public void Apply()
        {
            _applied = true;
            OnApply();
        }

        public abstract void OnApply();
        public abstract void OnComplete();
        public abstract void OnDispel();
        public abstract void OnCollision(StatusEffect ef);
        
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