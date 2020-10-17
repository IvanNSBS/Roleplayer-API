using Newtonsoft.Json.Linq;

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
    }
}