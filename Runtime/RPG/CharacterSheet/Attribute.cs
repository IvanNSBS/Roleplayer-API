
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class Attribute<T> where T : IComparable
    {
        #region Fields
        [JsonProperty] public readonly T defaultVal;
        [JsonProperty] public readonly T maxVal;
        protected T _value;
        protected T _modifiers;
        protected List<IAttributeModifier<T>> _flatMods;
        protected List<IAttributeModifier<T>> _percentMods;
        #endregion


        #region Properties
        public T Value => _value;
        public T Modifiers => _modifiers;
        public virtual T Total => Sum(_value, _modifiers);
        public IReadOnlyList<IAttributeModifier<T>> FlatMods => _flatMods;
        public IReadOnlyList<IAttributeModifier<T>> PercentMods => _percentMods;
        #endregion


        #region Constructor
        public Attribute(T defaultVal)
        {
            this.defaultVal = defaultVal;
            this.maxVal = DefaultMaxValue();
            _value = this.defaultVal;
            _flatMods = new List<IAttributeModifier<T>>();
            _percentMods = new List<IAttributeModifier<T>>();
        } 
            
        public Attribute(T defaultVal, T maxVal) 
        {
            this.maxVal = maxVal;
            this.defaultVal = defaultVal;
            _value = this.defaultVal;
            _flatMods = new List<IAttributeModifier<T>>();
            _percentMods = new List<IAttributeModifier<T>>();
        } 
        #endregion


        #region Methods
        public event Action<T> onValueChanged = delegate { };
        public event Action<T> onModifiersChanged = delegate { };

        public void Increase(T amount) 
        {
            _value = Sum(_value, amount);
            if(maxVal.CompareTo(DefaultMaxValue()) != 0)
                _value = Clamp(_value, defaultVal, maxVal);

            onValueChanged?.Invoke(_value);
        }

        public void Decrease(T amount) 
        {
            _value = Subtract(_value, amount);
            _value = Clamp(_value, defaultVal, Sum(_value, amount));

            onValueChanged?.Invoke(_value);
        }

        public virtual IAttributeModifier<T> AddFlatModifier(T amount)
        {
            IAttributeModifier<T> mod = new AttributeModifier<T>(amount); 
            _flatMods.Add(mod);

            _modifiers = CalculateModifiers();
            onModifiersChanged?.Invoke(_modifiers);
            return mod;
        }

        public virtual IAttributeModifier<T> AddPercentModifier(float pct)
        {
            IAttributeModifier<T> mod = new AttributeModifier<T>(Scale(pct));
            _percentMods.Add(mod);
            
            _modifiers = CalculateModifiers();
            onModifiersChanged?.Invoke(_modifiers);
            return mod;
        }

        public virtual bool RemoveFlatModifier(IAttributeModifier<T> mod)
        {
            bool removed = _flatMods.Remove(mod);
            if(removed) {
                _modifiers = CalculateModifiers();
                onModifiersChanged?.Invoke(_modifiers);
            }          
            return removed;
        }

        public virtual bool RemovePercentModifier(IAttributeModifier<T> mod)
        {
            bool removed = _percentMods.Remove(mod);
            if(removed) {
                _modifiers = CalculateModifiers();
                onModifiersChanged?.Invoke(_modifiers);
            }

            return removed;
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
            onModifiersChanged?.Invoke(_modifiers);
        }
        #endregion


        #region Interface Methods
        protected abstract T Scale(float b);
        protected abstract T Sum(T a, T b);
        protected abstract T Subtract(T a, T b);
        protected abstract T DefaultMaxValue();
        protected abstract T Zero();
        protected abstract T Clamp(T value, T min, T max);

        protected virtual T CalculateModifiers() => Zero();
        #endregion
    }
}