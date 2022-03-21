namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Class that contains general stats about the StatusEffect existence in the
    /// StatusEffectManager
    /// </summary>
    public class EffectApplyStats
    {
        #region Fields
        protected float _inactiveTime;
        protected int _timesApplied;
        protected float _secondsSinceFirstApply;
        protected float _secondsSinceLastApply;
        #endregion


        #region Properties
        /// <summary>
        /// Returns how much time, in seconds, the effect has not been active(applied) in the manager. 
        /// </summary>
        /// <value></value>
        public float InactiveTime 
        {
            get => _inactiveTime;
            set => _inactiveTime = value;
        } 

        /// <summary>
        /// How many times the effect has been applied
        /// </summary>
        /// <value></value>
        public int TimesApplied
        {
            get => _timesApplied;
            set => _timesApplied = value;
        }

        /// <summary>
        /// How many seconds has passed since the given effect was applied for the first time in the manager
        /// </summary>
        /// <value></value>
        public float SecondsSinceFirstApply
        {
            get => _secondsSinceFirstApply;
            set => _secondsSinceFirstApply = value;
        }

        /// <summary>
        /// How many seconds has passed since the effect was applied since the last apply/reapply was called
        /// </summary>
        /// <value></value>
        public float SecondsSinceLastApply
        {
            get => _secondsSinceLastApply;
            set => _secondsSinceLastApply = value;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Updates the Stats, given how many time has passed since the last frame
        /// </summary>
        /// <param name="deltaTime">How many times has passed since the last frame</param>
        /// <param name="isActive">Whether or not the watched effect is active in the manager</param>
        public void Update(float deltaTime, bool isActive) 
        {
            if(!isActive)
                _inactiveTime += deltaTime;

            _secondsSinceLastApply += deltaTime;
            _secondsSinceFirstApply += deltaTime;
        } 

        /// <summary>
        /// Completely resets the EffectApplyStats, zeroing all stats parameters
        /// </summary>
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