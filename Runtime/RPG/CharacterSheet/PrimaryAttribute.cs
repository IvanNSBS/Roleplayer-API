
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class PrimaryAttribute<T> : RPGAttribute<T> where T : IComparable
    {
        #region Constructors
        public PrimaryAttribute() : base() { } 
        public PrimaryAttribute(T defaultVal) : base(defaultVal) { } 
        public PrimaryAttribute(T defaultVal, T maxVal) : base(defaultVal, maxVal) { } 
        #endregion


        #region Methods
        public event Action<T> onValueChanged = delegate { };

        public void Increase(T amount) 
        {
            _value = Sum(_value, amount);
            if(maxVal.CompareTo(DefaultMaxValue()) != 0)
                _value = Clamp(_value, defaultVal, maxVal);

            onValueChanged?.Invoke(_value);
            OnAttributeChanged();
        }

        public void Decrease(T amount) 
        {
            _value = Subtract(_value, amount);
            _value = Clamp(_value, defaultVal, Sum(_value, amount));

            onValueChanged?.Invoke(_value);
            OnAttributeChanged();
        }

        /// <summary>
        /// Resets the attribute to the default value after construction
        /// </summary>
        public void Reset()
        {
            _value = defaultVal;
            _flatMods.Clear();
            _percentMods.Clear();
            _modifiers = CalculateModifiers();

            onValueChanged?.Invoke(_value);
            OnAttributeChanged();
        }
        #endregion


        #region Interface Methods
        protected abstract T Sum(T a, T b);
        protected abstract T Subtract(T a, T b);
        protected abstract T Clamp(T value, T min, T max);
        #endregion
    }
}