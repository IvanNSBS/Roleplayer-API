
using UnityEngine;
using Newtonsoft.Json;

namespace INUlib.RPG.CharacterSheet
{
    public class IntAttribute : PrimaryAttribute<int>
    {
        #region Constructor
        [JsonConstructor]
        public IntAttribute(int defaultVal) : base(defaultVal) { } 
        public IntAttribute(int defaultVal, int maxVal) : base(defaultVal, maxVal) { } 
        #endregion


        #region Methods
        public override int AsInt() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override float AsFloat() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
    
        protected override int Scale(float b) => Mathf.FloorToInt((float)_value * b);
        protected override int Sum(int a, int b) => a + b;
        protected override int Subtract(int a, int b) => a - b;
        protected override int DefaultMaxValue() => -1;
        protected override int Zero() => 0;
        protected override int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);
        #endregion
    }
}