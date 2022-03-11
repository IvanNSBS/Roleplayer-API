using System;

namespace INUlib.RPG.RPGAttributes
{
    public interface IAttributeMod
    {
        int ValueAsInt();
        float ValueAsFloat();
        void RefreshValue();
    }
}