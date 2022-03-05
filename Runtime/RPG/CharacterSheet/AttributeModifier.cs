
using System;
using UnityEngine;

namespace INUlib.RPG.CharacterSheet
{
    public class AttributeModifier<T> : IAttributeModifier<T> where T : IComparable
    {
        #region Fields
        private T _addedValue;
        private Attribute<T> _target;
        #endregion

        #region Properties
        public T Value => _addedValue;
        #endregion


        #region Constructor
        public AttributeModifier(T amount, Attribute<T> attr) => _addedValue = amount;
        public void Apply() => _target.Increase(_addedValue); 
        public void Remove() => _target.Decrease(_addedValue);
        #endregion
    }
}