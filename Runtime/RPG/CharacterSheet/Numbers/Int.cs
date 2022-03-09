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
        public override INumber Sum(INumber b)
        {
            _value += b.AsInt();
            return this;
        }

        public override INumber Sum(float b)
        {
            _value += (int)b;
            return this;
        }

        public override INumber Sum(int b)
        {
            _value += b;
            return this;
        }

        public override INumber Subtract(INumber b)
        {
            _value -= b.AsInt();
            return this;
        }

        public override INumber Subtract(float b)
        {
            _value -= (int)b;
            return this;
        }

        public override INumber Subtract(int b)
        {
            _value -= b;
            return this;
        }

        public override INumber Multiply(INumber b)
        {
            _value = (int)(_value * b.AsFloat());
            return this;
        }

        public override INumber Multiply(float b)
        {
            _value *= (int)b;
            return this;
        }

        public override INumber Multiply(int b)
        {
            _value *= b;
            return this;
        }
        
        public override INumber Divide(INumber b)
        {
            _value = (int)(_value / b.AsFloat());
            return this;
        }

        public override INumber Divide(float b)
        {
            _value /= (int)b;
            return this;
        }

        public override INumber Divide(int b)
        {
            _value /= b;
            return this;
        }

        public override int AsInt() => _value;
        public override float AsFloat() => _value;
        #endregion
    }
} 