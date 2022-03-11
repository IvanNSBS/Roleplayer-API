using UnityEngine;

namespace INUlib.RPG.RPGAttributes
{
    public class PrimaryIntAttribute : RPGAttribute
    {
        #region Constructors
        public PrimaryIntAttribute() : base(AttributeType.Integer) { }
        public PrimaryIntAttribute(float dfVal) : base(AttributeType.Integer, dfVal) { }
        public PrimaryIntAttribute(float dfVal, float maxVal) : base(AttributeType.Integer, dfVal, maxVal) { }
        #endregion


        #region Methods
        public override float Total => (int)_currentValue + (int)_modsValue;
        
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