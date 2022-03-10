using System;

namespace INUlib.RPG.CharacterSheet
{
    public interface IAttribute
    {
        int AsInt();
        float AsFloat();
        event Action onAttributeChanged;
    }
}