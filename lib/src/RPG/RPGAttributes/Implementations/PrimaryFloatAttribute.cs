namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// PrimaryAttribute that is created with Float type
    /// and calculates the mods using their sum of float values
    /// </summary>
    public class PrimaryFloatAttribute : PrimaryAttribute
    {
        #region Constructors
        /// <summary>
        /// Creates the Attribute with defaultValue as 0 and no maxValue
        /// </summary>
        public PrimaryFloatAttribute() : base(AttributeType.Float) { }
        
        /// <summary>
        /// Creates the Attribute with a given default value
        /// </summary>
        /// <param name="dfVal">The default attribute value</param>
        public PrimaryFloatAttribute(float dfVal, float minVal) : base(AttributeType.Float, dfVal, minVal) { }
        
        /// <summary>
        /// Creates the attribute with a given default and max value
        /// </summary>
        /// <param name="dfVal">The default attribute value</param>
        /// <param name="maxVal">The default attribute max value</param>
        public PrimaryFloatAttribute(float dfVal, float minVal, float maxVal) : base(AttributeType.Float, dfVal, minVal, maxVal) { }
        #endregion


        #region Methods
        public override float Total => _currentValue + _modsValue;
        
        /// <summary>
        /// Calculte the Attribute Modifiers, returning the sum of all Percent and Flat mods
        /// </summary>
        /// <returns>The newly calculated ModsValue</returns>
        public override float CalculateMods()
        {
            float total = 0;
            foreach(var pctMod in _percentMods)
                total += pctMod.ValueAsFloat();
            foreach(var flatMod in _flatMods)
                total += flatMod.ValueAsFloat();
        
            return total;
        }
        #endregion
    }
}