
using UnityEngine;
using Newtonsoft.Json;

namespace INUlib.RPG.CharacterSheet
{
    public class FloatAttribute : Attribute<float>
    {
        #region Constructor
        [JsonConstructor]
        public FloatAttribute(float defaultVal) : base(defaultVal) { } 
        public FloatAttribute(float defaultVal, float maxVal) : base(defaultVal, maxVal) { } 
        #endregion


        #region Methods
        protected override float Scale(float b) => _value * b;
        protected override float Sum(float a, float b) => a + b;
        protected override float Subtract(float a, float b) => a - b;
        protected override float DefaultMaxValue() => -1;
        protected override float Zero() => 0;
        protected override float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);
        #endregion
    }
}