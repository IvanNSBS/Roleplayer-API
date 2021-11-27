using System;

namespace Common.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string enumName) where T : Enum
        {
            return (T) Enum.Parse(typeof(T), enumName);
        }
    }
}