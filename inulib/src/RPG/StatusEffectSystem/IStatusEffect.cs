namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Base interface for all status effects
    /// </summary>
    public interface IStatusEffect
    {
        /// <summary>
        /// Defines how to apply a status effect, given the Apply Stats
        /// </summary>
        /// <param name="stats">The ApplyStats</param>
        void Apply(EffectApplyStats stats);

        /// <summary>
        /// Status Effect behaviour when it completes
        /// </summary>
        void OnComplete();

        /// <summary>
        /// Status Effect behaviour when it is dispeled(removed prematurely)
        /// </summary>
        void OnDispel();

        /// <summary>
        /// What to do when the Status Effect is reapplied
        /// </summary>
        /// <param name="ef">The StatusEffect that that will trigger the reapply</param>
        /// <param name="stats">The ApplyStats</param>
        void Reapply(IStatusEffect ef, EffectApplyStats stats);

        /// <summary>
        /// Function to run on the frame update
        /// </summary>
        /// <param name="deltaTime">The time since the last frame</param>
        /// <returns>True if the effect has been completed. False otherwise</returns>
        bool Update(float deltaTime);
    }
}