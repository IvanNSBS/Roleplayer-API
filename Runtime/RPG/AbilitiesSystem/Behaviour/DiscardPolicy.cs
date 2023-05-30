namespace INUlib.RPG.AbilitiesSystem
{
    public enum DiscardPolicy
    {
        /// <summary>
        /// Ability will be discarded once the timeline and recovery finishes
        /// </summary>
        AfterRecovery = 0,
        
        /// <summary>
        /// Ability will be discarded after casting if the ability is not concentration or after
        /// concentrating if it's a concentrating ability
        /// </summary>
        AfterCastingOrConcentrating = 1,
        
        /// <summary>
        /// Ability will be discarded only when the AbilityObject manually calls NotifyDiscard from withing
        /// </summary>
        Manual = 2,
    }
}