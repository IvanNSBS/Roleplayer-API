namespace INUlib.RPG.RPGAttributes
{
    public abstract class PrimaryAttribute : RPGAttribute
    {
        #region Constructors
        public PrimaryAttribute(AttributeType t) : base(t) { }
        public PrimaryAttribute(AttributeType t, float dfVal) : base(t, dfVal) { }
        public PrimaryAttribute(AttributeType t, float dfVal, float maxVal) : base(t, dfVal, maxVal) { }
        #endregion


        #region Methods
        public void Increase(float amount)
        {
            if(_type == AttributeType.Integer)
                _currentValue += (int)amount;
            else
                _currentValue += amount;

            RaiseAttributeChanged();
        }

        public void Increase(int amount)
        {
            _currentValue += amount;
            RaiseAttributeChanged();
        }

        public void Decrease(float amount)
        {
            if(_type == AttributeType.Integer)
                _currentValue -= (int)amount;
            else
                _currentValue -= amount;
        
            RaiseAttributeChanged();
        }

        public void Decrease(int amount)
        {
            _currentValue -= amount;
            RaiseAttributeChanged();
        }
        #endregion
    }
}