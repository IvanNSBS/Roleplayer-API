using System;

namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Interface for a Character Attribute stats
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        /// Gets the current Attribute value as an integer
        /// </summary>
        /// <returns></returns>
        int ValueAsInt();

        /// <summary>
        /// Gets the current Attribute valeu as a float
        /// </summary>
        /// <returns></returns>
        float ValueAsFloat();

        /// <summary>
        /// Event to fire whenever the attribute changes it's value
        /// </summary>
        event Action onAttributeChanged;
    }
}