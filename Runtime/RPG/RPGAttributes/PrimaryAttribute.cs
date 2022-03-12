namespace INUlib.RPG.RPGAttributes
{
    /// <summary>
    /// Defines a Primary Attribute, such as INT, DEX or STR. It's a RPGAttribute Implementation
    /// PrimaryAttributes can be Increased or Decreased, changing it's intrinsic CurrentValue
    /// A PrimaryAttribute can't be decreased past the default value, and if the maxValue was set,
    /// won't be able to be increased past it.
    /// </summary>
    public abstract class PrimaryAttribute : RPGAttribute
    {
        #region Constructors
        /// <summary>
        /// Creates the PrimaryAttribute given it's type, but with the default value as 0 and no max value
        /// </summary>
        /// <param name="t">The Attribute Math Type</param>
        public PrimaryAttribute(AttributeType t) : base(t) { }

        /// <summary>
        /// Creates the Primary attribute, given it's type and a default value
        /// </summary>
        /// <param name="t"The Attribute Math Type></param>
        /// <param name="dfVal">the Attribute default value</param>
        public PrimaryAttribute(AttributeType t, float dfVal) : base(t, dfVal) { }

        /// <summary>
        /// Creates the primary attribute, with a type, default and max value
        /// </summary>
        /// <param name="t">The attribute Math Type</param>
        /// <param name="dfVal">The Attribute default value</param>
        /// <param name="maxVal">The attribute max value</param>
        public PrimaryAttribute(AttributeType t, float dfVal, float maxVal) : base(t, dfVal, maxVal) { }
        #endregion


        #region Methods
        /// <summary>
        /// Increases the attribute currentValue by the given real value.
        /// If maxValue is set, currentValue will be clamped to maxValue
        /// If the AttributeType is Integer, the real value will be truncated;
        /// It will refresh the Attribute mods. Fires onAttributeChanged.
        /// </summary>
        /// <param name="amount">The amount to Increase</param>
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

        /// <summary>
        /// Increases the attribute currentValue by the given value.
        /// If maxValue is set, currentValue will be clamped to maxValue
        /// It will refresh the Attribute mods. Fires onAttributeChanged.
        /// </summary>
        /// <param name="amount">The amount to Increase</param>
        public virtual void Increase(int amount)
        {
            _currentValue += amount;
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }

        /// <summary>
        /// Decreases the attribute currentValue by the given real value.
        /// If the AttributeType is Integer, the value will be truncated
        /// The currentValue will be clamped to the defaultValue when decreasing.
        /// It will refresh the Attribute mods. Fires onAttributeChanged.
        /// </summary>
        /// <param name="amount">The amount to Increase</param>
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

        /// <summary>
        /// Decreases the attribute currentValue by the given real value.
        /// If the AttributeType is Integer, the value will be truncated
        /// The currentValue will be clamped to the defaultValue when decreasing.
        /// It will refresh the Attribute mods. Fires onAttributeChanged.
        /// </summary>
        /// <param name="amount">The amount to Increase</param>
        public virtual void Decrease(int amount)
        {
            _currentValue -= amount;
            ClampCurrentValue();
            RefreshMods();
            RaiseAttributeChanged();
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper method to clamp the currentValue to the defaultValue 
        /// and limit it to the maxValue, if it's set
        /// </summary>
        protected void ClampCurrentValue()
        {
            if(_currentValue < defaultValue)
                _currentValue = defaultValue;
            if(maxValue != -1 && _currentValue > maxValue)
                _currentValue = maxValue;
        }
        #endregion
    }
}