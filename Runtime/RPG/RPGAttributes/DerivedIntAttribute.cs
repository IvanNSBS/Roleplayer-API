namespace INUlib.RPG.RPGAttributes
{
    public abstract class DerivedIntAttribute : DerivedAttribute
    {
        #region Constructors
        public DerivedIntAttribute() : base(AttributeType.Integer) { }
        public DerivedIntAttribute(int dfVal, int minVal) : base(AttributeType.Integer, dfVal, minVal) { }
        public DerivedIntAttribute(int dfVal, int minVal, int maxVal) : base(AttributeType.Integer, dfVal, minVal, maxVal) { }
        #endregion


        #region Methods
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