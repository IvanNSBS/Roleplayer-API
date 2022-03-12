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
        public virtual void Increase(float amount)
        {
            if(_type == AttributeType.Integer)
                _currentValue += (int)amount;
            else
                _currentValue += amount;
 
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }

        public virtual void Increase(int amount)
        {
            _currentValue += amount;
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }

        public virtual void Decrease(float amount)
        {
            if(_type == AttributeType.Integer)
                _currentValue -= (int)amount;
            else
                _currentValue -= amount;
            
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }

        public virtual void Decrease(int amount)
        {
            _currentValue -= amount;
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }
        #endregion


        #region Helper Methods
        protected void ClampCurrentValue()
        {
            if(_currentValue < defaultValue)
                _currentValue = defaultValue;
            if(maxValue != -1 && _currentValue > maxValue)
                _currentValue = maxValue;
        }

        protected void RefreshMods()
        {
            foreach(IAttributeMod flatMod in _flatMods)
                flatMod.RefreshValue();
            foreach(IAttributeMod pctMod in _percentMods)
                pctMod.RefreshValue();
        }
        #endregion
    }
}