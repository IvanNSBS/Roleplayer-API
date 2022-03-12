using System;

namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Implementation of a IAttributeMod.
    /// Calculates a percentage of the attribute value
    /// </summary>
    public class PercentAttributeMod : IAttributeMod
    {
        #region Fields
        private bool _truncate;
        private float _pct;
        private float _value;
        private Func<float> _attrGetter;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the PercentAttribute mod.
        /// </summary>
        /// <param name="pct">The percent value of the modifier</param>
        /// <param name="attrGetter">Getter function that returns the Attribute value, so the Mod can calculate it's value</param>
        /// <param name="truncate">Wheter or not the PercentAttribute should truncate the calculate the value</param>
        public PercentAttributeMod(float pct, Func<float> attrGetter, bool truncate)
        {
            _truncate = truncate;
            _pct = pct;
            _attrGetter = attrGetter;
            RefreshValue();
        } 
        #endregion


        #region Methods
        public int ValueAsInt() => (int)_value;
        public float ValueAsFloat() => _value;

        /// <summary>
        /// Refreshse the value, recalculating the PercentMod value using the AttributeGetter given in 
        /// the constructor
        /// </summary>
        public void RefreshValue()
        {
            if(_truncate)
                _value = (int)(_attrGetter() * _pct);
            else
                _value = _attrGetter() * _pct; 
        } 
            
        #endregion
    }
}