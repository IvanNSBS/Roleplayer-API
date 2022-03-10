
using UnityEngine;
using Newtonsoft.Json;

namespace INUlib.RPG.CharacterSheet
{
    public class FloatAttribute : PrimaryAttribute<float>
    {
        #region Constructor
        [JsonConstructor]
        public FloatAttribute(float defaultVal) : base(defaultVal) { } 
        public FloatAttribute(float defaultVal, float maxVal) : base(defaultVal, maxVal) { } 
        #endregion


        #region Methods
        public override int AsInt() => (int)Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override float AsFloat() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));

        protected override float Scale(float b) => _value * b;
        protected override float Sum(float a, float b) => a + b;
        protected override float Subtract(float a, float b) => a - b;
        protected override float DefaultMaxValue() => -1;
        protected override float Zero() => 0;
        protected override float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);
        #endregion
    }
}