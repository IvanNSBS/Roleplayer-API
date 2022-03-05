
using System;

namespace INUlib.RPG.CharacterSheet
{
    public class AttributeModifier<T> : IAttributeModifier<T> where T : IComparable
    {
        #region Fields
        private T _addedValue;
        #endregion

        #region Properties
        public T Value => _addedValue;
        #endregion


        #region Constructor
        public AttributeModifier(T amount) => _addedValue = amount;
        #endregion
    }
}