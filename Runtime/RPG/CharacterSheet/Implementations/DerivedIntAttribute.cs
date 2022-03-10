using UnityEngine;

namespace INUlib.RPG.CharacterSheet
{
    public abstract class DerivedIntAttribute : DerivedAttribute<int>
    {
        #region Constructor
        public DerivedIntAttribute() : base() { } 
        ~DerivedIntAttribute() => UnlinkParents();
        #endregion


        #region Methods
        public override int Total => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override int AsInt() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        public override float AsFloat() => Clamp(_value + _modifiers, defaultVal, Mathf.Abs(_value + _modifiers));
        protected override int Scale(float b) => Mathf.FloorToInt((float)_value * b);
        protected override int Zero() => 0;
        protected override int DefaultMaxValue() => -1;
        protected int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);
        #endregion
    }
}