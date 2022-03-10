using UnityEngine;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedFloatAttribute : DerivedAttribute<float>
    {
        #region Constructor
        public DerivedFloatAttribute() : base() { } 
        ~DerivedFloatAttribute() => UnlinkParents();
        #endregion


        #region Methods
        public override float Total => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override int AsInt() => (int)Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override float AsFloat() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        protected override float Scale(float b) => _value * b;
        protected override float Zero() => 0;
        protected override float DefaultMaxValue() => -1;
        protected float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);
        #endregion
    }
}