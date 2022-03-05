
using System;

namespace INUlib.RPG.CharacterSheet
{
    public interface IAttributeModifier<T> where T : IComparable
    {
        T Value { get; }
        void Apply();
        void Remove();
    }
}