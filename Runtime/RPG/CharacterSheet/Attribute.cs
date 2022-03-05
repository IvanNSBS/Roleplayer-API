
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
        protected List<IAttributeModifier<T>> _flatBonuses;
        protected List<IAttributeModifier<T>> _percentBonuses;
        #endregion


        #region Properties
        public T DefaultValue => defaultVal;
        public T Value => _value;
        #endregion


        #region Constructor
        public Attribute(T defaultVal)
        {
            this.defaultVal = defaultVal;
            _value = this.defaultVal;
            _flatBonuses = new List<IAttributeModifier<T>>();
            _percentBonuses = new List<IAttributeModifier<T>>();
        } 
            
        public Attribute(T defaultVal, T maxVal) 
        {
            this.maxVal = maxVal;
            this.defaultVal = defaultVal;
            _value = this.defaultVal;
            _flatBonuses = new List<IAttributeModifier<T>>();
            _percentBonuses = new List<IAttributeModifier<T>>();
        } 
        #endregion


        #region Methods
        public event Action<T> onValueChanged = delegate { };
        public event Action<T> onModifiersChanged = delegate { };

        public void Increase(T amount) 
        {
            _value = Sum(_value, amount);
            if(maxVal.CompareTo(DefaultMaxValue()) == 0)
                _value = Clamp(_value, defaultVal, maxVal);

            onValueChanged?.Invoke(_value);
        }

        public void Decrease(T amount) 
        {
            _value = Subtract(_value, amount);
            T clampVal = maxVal.CompareTo(DefaultMaxValue()) == 0 ? _value : maxVal; 
            _value = Clamp(_value, defaultVal, clampVal);

            onValueChanged?.Invoke(_value);
        }

        public virtual IAttributeModifier<T> AddFlatModifier(T amount)
        {
            IAttributeModifier<T> mod = new AttributeModifier<T>(amount, this); 
            mod.Apply();
            _flatBonuses.Add(mod);

            return mod;
        }

        public virtual bool RemoveFlatModifier(IAttributeModifier<T> mod)
        {
            bool removed = _flatBonuses.Remove(mod);
            if(removed)
                mod.Remove();

            return removed;
        }

        public virtual IAttributeModifier<T> AddPercentModifier(float pct)
        {
            float clampedPct = Mathf.Clamp01(pct);
            IAttributeModifier<T> mod = new AttributeModifier<T>(Scale(clampedPct), this);
            mod.Apply();
            _percentBonuses.Add(mod);

            return mod;
        }

        public virtual bool RemovePercentModifier(IAttributeModifier<T> mod)
        {
            bool removed = _percentBonuses.Remove(mod);
            if(removed)
                mod.Remove();

            return removed;
        }

        public void Reset()
        {
            _value = defaultVal;
            _percentBonuses.Clear();
            _flatBonuses.Clear();
            onValueChanged?.Invoke(_value);
        }
        #endregion


        #region Interface Methods
        protected abstract T Scale(float b);
        protected abstract T Sum(T a, T b);
        protected abstract T Subtract(T a, T b);
        protected abstract T DefaultMaxValue();
        protected abstract T Clamp(T value, T min, T max);

        protected virtual void ApplyBonuses() { }
        #endregion
    }
}