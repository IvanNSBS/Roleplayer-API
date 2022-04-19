namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Defines the Cast Handler behaviour when it receives requests from the user, passing the 
    /// information to the AbilityObject
    /// </summary>
    public abstract class CastHandlerPolicy
    {
        #region Fields
        protected float castTimer;
        private IAbilityObject _abilityObj;
        #endregion


        #region Constructors
        #endregion


        #region Methods
        /// <summary>
        /// Updates the elapsed cast time and calls te OnUpdate event for the CastHandler Policy
        /// </summary>
        /// <param name="deltaTime">How much time has passed since the last frame</param>
        public void Update(float deltaTime) 
        {
            castTimer += deltaTime;
            OnUpdate();
        }

        /// <summary>
        /// Defines the behavior for the CastHandler update
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Behaviour for when the user calls the cast function
        /// </summary>
        /// <param name="castAmount">How many times the cast has been called so far</param>
        /// <param name="state">The current casting state for the ability</param>
        public abstract void OnCastRequested(int castAmount, CastingState state);

        /// <summary>
        /// Behaviour for when the user cancels the channeling/cast of the spell 
        /// </summary>
        /// <param name="state">The state that the cast was in</param>
        public abstract void OnCancelRequested(CastingState state);
        #endregion
    }
}
