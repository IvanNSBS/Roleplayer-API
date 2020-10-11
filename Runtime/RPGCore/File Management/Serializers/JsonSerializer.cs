using System.IO;
using Newtonsoft.Json;

namespace RPGCore.DataManagement.Serializers
{
    public static class JsonSerializer
    {
        public static void ToJson(object obj, string filePath)
        {
            string output = JsonConvert.SerializeObject(obj);
            File.WriteAllText(Path.Combine(filePath, $"{obj.ToString()}.json"), output);
        }

        public static void ToJsonFile(this string jsonString, string filePath)
        {
            File.WriteAllText(Path.Combine(filePath), jsonString);
        }
    }
}