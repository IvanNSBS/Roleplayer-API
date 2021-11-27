using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JetBrains.Annotations;

namespace Common.Utils.Extensions
{
    public static class DotNetJson
    {
        public static void ToFile(this JObject jObject, string filePath, Formatting formatting = Formatting.Indented)
        {
            File.WriteAllText(Path.Combine(filePath), jObject.ToString(formatting));
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
    }
}