namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Implementation of a IAttributeMod.
    /// Simply holds the given, since it'll be a flat modifier.
    /// </summary>
    public class FlatAttributeMod : IAttributeMod
    {
        #region Fields
        private float _value;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the Modifier from an integer
        /// </summary>
        /// <param name="value">The attribute value</param>
        public FlatAttributeMod(int value) => _value = value;

        /// <summary>
        /// Creates the Modifier from a float
        /// </summary>
        /// <param name="value">The attribute value</param>
        public FlatAttributeMod(float value) => _value = value;
        #endregion


        #region Methods
        public int ValueAsInt() => (int)_value;
        public float ValueAsFloat() => _value;
        
        /// <summary>
        /// FlatAttribute does not need to refresh it's value.
        /// </summary>
        public void RefreshValue() { }
        #endregion
    }
}