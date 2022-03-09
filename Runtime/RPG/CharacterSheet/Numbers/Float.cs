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
        public override INumber Sum(INumber b)
        {
            _value += b.AsFloat();
            return this;
        }

        public override INumber Sum(float b)
        {
            _value += b;
            return this;
        }

        public override INumber Sum(int b)
        {
            _value += b;
            return this;
        }

        public override INumber Subtract(INumber b)
        {
            _value -= b.AsFloat();
            return this;
        }

        public override INumber Subtract(float b, bool preceding)
        {
            if(preceding)
                _value = b - _value;
            else
                _value -= b;
            return this;
        }

        public override INumber Subtract(int b, bool preceding)
        {
            if(preceding)
                _value = b - _value;
            else
                _value -= b;
            return this;
        }

        public override INumber Multiply(INumber b)
        {
            _value = _value * b.AsFloat();
            return this;
        }

        public override INumber Multiply(float b)
        {
            _value *= b;
            return this;
        }

        public override INumber Multiply(int b)
        {
            _value *= b;
            return this;
        }
        
        public override INumber Divide(INumber b)
        {
            _value = _value / b.AsFloat();
            return this;
        }

        public override INumber Divide(float b, bool preceding)
        {
            if(preceding)
                _value = b / _value;
            else
                _value /= b;
            return this;
        }

        public override INumber Divide(int b, bool preceding)
        {
            if(preceding)
                _value = (float)b / _value;
            else
                _value /= b;
            return this;
        }

        public override int AsInt() => (int)_value;
        public override float AsFloat() => _value;
        #endregion
    }
} 