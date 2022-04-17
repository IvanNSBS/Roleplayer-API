namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Defines the Cast Handler behaviour when it receives requests from the user, passing the 
    /// information to the AbilityObject
    /// </summary>
    public abstract class CastHandlerPolicy<IAbilityObject, TCaster> 
           where IAbilityObject : class, IAbilityObject<TCaster> where TCaster : IAbilityCaster
    {
        #region Fields
        protected float castTimer;
        private IAbilityObject<TCaster> _abilityObj;
        #endregion


        #region Constructors
        #endregion


        #region Methods
        public void Update(float deltaTime) => castTimer += deltaTime;
        protected abstract void OnUpdate();

        public virtual void OnChannelingCompleted() { }
        public abstract void OnCastRequested(int castAmount, CastingState state);
        public abstract void OnStopCastRequested(CastingState state);
        #endregion
    }
}
