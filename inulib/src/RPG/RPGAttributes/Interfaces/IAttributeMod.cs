namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Interface for a Attirbute Modifier.
    /// Attribute Modifiers are value modifications that come from an external source, like a bonus
    /// given from an item or buff
    /// </summary>
    public interface IAttributeMod
    {
        /// <summary>
        /// /// Gets the AttributeMod value as an integer
        /// </summary>
        /// <returns></returns>
        int ValueAsInt();
        /// <summary>
        /// Gets the AttributeMod value as a float
        /// </summary>
        /// <returns></returns>
        /// 
        float ValueAsFloat();
        
        /// <summary>
        /// Function that will be called when the mod parent changes. Useful for 
        /// mods that affect a percentage of the attribute value
        /// </summary>
        void RefreshValue();
    }
}