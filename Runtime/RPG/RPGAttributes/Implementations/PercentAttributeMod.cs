using System;

namespace INUlib.RPG.RPGAttributes
{
    public class PercentAttributeMod : IAttributeMod
    {
        #region Fields
        private bool _truncate;
        private float _pct;
        private float _value;
        private Func<float> _attrGetter;
        #endregion

        #region Constructor
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