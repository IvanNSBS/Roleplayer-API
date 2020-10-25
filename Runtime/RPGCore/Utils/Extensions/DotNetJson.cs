using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JetBrains.Annotations;

namespace RPGCore.Utils.Extensions
{
    public static class DotNetJson
    {
        public static void AddOrUpdate(this JObject jObject, string propertyName, JToken value)
        {
            if (jObject.ContainsKey(propertyName))
                jObject[propertyName] = value;
            else
                jObject.Add(propertyName, value);
        }
        
        public static void AddOrUpdate([CanBeNull] this JToken jObject, string propertyName, JToken value)
        {
            if (jObject == null)
                return;
            
            if (jObject.Value<JObject>().ContainsKey(propertyName))
                jObject[propertyName] = value;
            else
                jObject.Value<JObject>().Add(propertyName, value);
        }

        /// <summary>
        /// Adds property to json if it don't exist
        /// </summary>
        /// <param name="token"> Jtoken to add jobject to</param>
        /// <param name="propertyName">property key</param>
        /// <param name="value">new value to add</param>
        public static void TryAdd([CanBeNull] this JToken token, string propertyName, JObject value)
        {
            if (token == null)
                return;
            
            var jobject = token.Value<JObject>();
            
            if(!jobject.ContainsKey(propertyName))
                jobject.Add(propertyName, value);
        }
    }
}