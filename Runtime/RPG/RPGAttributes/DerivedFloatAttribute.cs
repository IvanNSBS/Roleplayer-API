namespace INUlib.RPG.RPGAttributes
{
    public abstract class DerivedFloatAttribute : RPGAttribute
    {
        #region Constructors
        public DerivedFloatAttribute() : base(AttributeType.Float) { }
        public DerivedFloatAttribute(float dfVal) : base(AttributeType.Float, dfVal) { }
        public DerivedFloatAttribute(float dfVal, float maxVal) : base(AttributeType.Float, dfVal, maxVal) { }
        #endregion
    }
}