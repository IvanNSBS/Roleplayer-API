namespace INUlib.RPG.AbilitiesSystem
{
    public enum DiscardPolicy
    {
        /// <summary>
        /// Ability will be discarded once the timeline and recovery finishes
        /// </summary>
        Auto = 0,
        
        /// <summary>
        /// Ability will be discarded only when the AbilityObject manually calls NotifyDiscard from withing
        /// </summary>
        Manual = 2,
    }
}