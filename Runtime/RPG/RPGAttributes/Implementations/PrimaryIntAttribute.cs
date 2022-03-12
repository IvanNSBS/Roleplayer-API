namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// PrimaryAttribute that is created with Integer type
    /// and calculates the mods using their sum of integer value
    /// </summary>
    public class PrimaryIntAttribute : RPGAttribute
    {
        #region Constructors
        /// <summary>
        /// Creates the Attribute with defaultValue as 0 and no maxValue
        /// </summary>
        public PrimaryIntAttribute() : base(AttributeType.Integer) { }

        /// <summary>
        /// Creates the Attribute with a given default value
        /// </summary>
        /// <param name="dfVal">The default attribute value</param>
        public PrimaryIntAttribute(float dfVal) : base(AttributeType.Integer, dfVal) { }

        /// <summary>
        /// Creates the attribute with a given default and max value
        /// </summary>
        /// <param name="dfVal">The default attribute value</param>
        /// <param name="maxVal">The default attribute max value</param>
        public PrimaryIntAttribute(float dfVal, float maxVal) : base(AttributeType.Integer, dfVal, maxVal) { }
        #endregion


        #region Methods
        /// <summary>
        /// Returns the total truncated value of currentValue and modsValue
        /// </summary>
        /// <returns></returns>
        public override float Total => (int)_currentValue + (int)_modsValue;
        
        /// <summary>
        /// Calculte the Attribute Modifiers, returning the sum of all Percent and Flat mods as integers
        /// </summary>
        /// <returns>The newly calculated ModsValue</returns>
        public override float CalculateMods()
        {
            int total = 0;
            foreach(var pctMod in _percentMods)
                total += pctMod.ValueAsInt();
            foreach(var flatMod in _flatMods)
                total += flatMod.ValueAsInt();
        
            if(total < 0)
                total = 0;

            return total;
        }
        #endregion
    }
}