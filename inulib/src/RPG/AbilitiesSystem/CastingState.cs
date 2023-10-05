namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Enumerator that contains the current casting state of a ability
    /// </summary>
    public enum CastingState
    {
        ///<summary>The ability is not being cast</summary>
        None = 0,
        
        ///<summary>The ability is being channeled and will be cast after it's channeled</summary>
        Channeling = 1,
        
        /// <summary> The ability finished channeling and is now overchanneling to increase the effect</summary>
        OverChanneling = 2,
        
        ///<summary>The ability has been fully chanelled and the effect is being unleashed</summary>
        Casting = 3,
        
        /// <summary>
        /// The ability was cast and the user is concentrating on it to keep the effect up. Only used
        /// with concentration spells
        /// </summary>
        Concentrating = 4,
        
        /// <summary>
        /// The ability has finished casting and concentrating and its finishing
        /// </summary>
        CastRecovery = 5
    }
}