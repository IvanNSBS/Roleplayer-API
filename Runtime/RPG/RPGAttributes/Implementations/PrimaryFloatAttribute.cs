using UnityEngine;

namespace INUlib.RPG.RPGAttributes
{
    public class PrimaryFloatAttribute : RPGAttribute
    {
        #region Constructors
        public PrimaryFloatAttribute() : base(AttributeType.Float) { }
        public PrimaryFloatAttribute(float dfVal) : base(AttributeType.Float, dfVal) { }
        public PrimaryFloatAttribute(float dfVal, float maxVal) : base(AttributeType.Float, dfVal, maxVal) { }
        #endregion


        #region Methods
        public override float Total => _currentValue + _modsValue;
        
        public override float CalculateMods()
        {
            float total = 0;
            foreach(var pctMod in _percentMods)
                total += pctMod.ValueAsFloat();
            foreach(var flatMod in _flatMods)
                total += flatMod.ValueAsFloat();
        
            if(total < 0)
                total = 0;

            return total;
        }
        #endregion
    }
}