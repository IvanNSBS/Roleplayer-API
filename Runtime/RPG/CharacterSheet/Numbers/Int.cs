namespace INUlib.RPG.CharacterSheet
{
    public class Int : INumber
    {
        #region Fields
        private int _value;
        #endregion


        #region Constructors
        public Int(int value) => _value = value;
        public Int() => _value = 0;
        #endregion


        #region Methods
        public static implicit operator Int(int num) => new Int(num);

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
            _value = (int)(_value*(float)b);
            return this;
        }
        
        public override INumber Divide(INumber b)
        {
            _value = (int)(_value/(float)b);
            return this;
        }

        protected override int AsInt() => _value;
        protected override float AsFloat() => _value;
        #endregion
    }
} 