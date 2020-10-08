using System;

namespace RPGCore.Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index)
        {
            int length = data.Length-index;
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}