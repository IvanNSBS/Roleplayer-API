namespace INUlib.RPG.RPGAttributes
{
    public abstract class DerivedFloatAttribute : DerivedAttribute
    {
        #region Constructors
        public DerivedFloatAttribute() : base(AttributeType.Float) { }
        public DerivedFloatAttribute(float dfVal, float minVal) : base(AttributeType.Float, dfVal, minVal) { }
        public DerivedFloatAttribute(float dfVal, float minVal, float maxVal) : base(AttributeType.Float, dfVal, minVal, maxVal) { }
        #endregion


        #region Methods
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