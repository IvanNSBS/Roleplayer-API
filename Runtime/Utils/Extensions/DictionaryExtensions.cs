using System.Collections.Generic;

namespace INUlib.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if(dict.ContainsKey(key))
                return dict[key];

            return default(TValue);
        }
    }
}
