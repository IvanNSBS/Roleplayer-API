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
        ///<summary>The ability has been fully chanelled and the effect is being unleashed</summary>
        Casting = 2
    }
}