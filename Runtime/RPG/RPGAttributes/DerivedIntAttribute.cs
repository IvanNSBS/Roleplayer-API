namespace INUlib.RPG.RPGAttributes
{
    public abstract class DerivedIntAttribute : RPGAttribute
    {
        #region Constructors
        public DerivedIntAttribute() : base(AttributeType.Integer) { }
        public DerivedIntAttribute(int dfVal) : base(AttributeType.Integer, dfVal) { }
        public DerivedIntAttribute(int dfVal, int maxVal) : base(AttributeType.Integer, dfVal, maxVal) { }
        #endregion
    }
}