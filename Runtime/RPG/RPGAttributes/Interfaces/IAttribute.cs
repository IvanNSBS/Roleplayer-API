using System;

namespace INUlib.RPG.RPGAttributes
{
    public interface IAttribute
    {
        int ValueAsInt();
        float ValueAsFloat();
        event Action onAttributeChanged;
    }
}