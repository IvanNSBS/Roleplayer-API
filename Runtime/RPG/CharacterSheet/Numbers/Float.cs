namespace INUlib.RPG.CharacterSheet
{
    public class Float : INumber
    {
        #region Fields
        private float _value;
        #endregion

        #region Constructors
        public Float(float value) => _value = value;
        public Float() => _value = 0;
        #endregion

        #region Methods
        public static implicit operator Float(float num) => new Float(num);

        public override INumber Sum(INumber b) 
        {
            _value += b;
            return this;
        }

        public override INumber Subtract(INumber b) 
        {
            _value -= b;
            return this;
        }
        public override INumber Multiply(INumber b)
        {
            _value *= b;
            return this;
        }
        
        public override INumber Divide(INumber b)
        {
            _value /= b;
            return this;
        }

        protected override int AsInt() => (int)_value;
        protected override float AsFloat() => _value;
        #endregion
    }
} 