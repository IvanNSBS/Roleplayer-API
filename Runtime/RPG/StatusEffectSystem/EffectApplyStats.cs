namespace INUlib.RPG.StatusEffectSystem
{
    public class EffectApplyStats
    {
        #region Fields
        protected float _inactiveTime;
        protected int _timesApplied;
        protected float _secondsSinceFirstApply;
        protected float _secondsSinceLastApply;
        #endregion


        #region Properties
        public float InactiveTime 
        {
            get => _inactiveTime;
            set => _inactiveTime = value;
        } 

        public int TimesApplied
        {
            get => _timesApplied;
            set => _timesApplied = value;
        }

        public float SecondsSinceFirstApply
        {
            get => _secondsSinceFirstApply;
            set => _secondsSinceFirstApply = value;
        }

        public float SecondsSinceLastApply
        {
            get => _secondsSinceLastApply;
            set => _secondsSinceLastApply = value;
        }
        #endregion


        #region Constructors
        public EffectApplyStats()
        {
        }
        #endregion


        #region Methods
        public void Update(float deltaTime, bool isActive) 
        {
            if(!isActive)
                _inactiveTime += deltaTime;

            _secondsSinceLastApply += deltaTime;
            _secondsSinceFirstApply += deltaTime;
        } 

        public void Reset()
        {
            _inactiveTime = 0;
            _timesApplied = 0;
            _secondsSinceLastApply = 0;
            _secondsSinceFirstApply = 0;
        }
        #endregion
    }
}