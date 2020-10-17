using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGCore.DataManagement.Serializers;

namespace RPGCore.FileManagement.SavingFramework.JsonEncryption
{
    /// <summary>
    /// NoEncryption is the default encryption for save files.
    /// It's here just to keep the architecture flow going without
    /// writing complex algorithms
    /// </summary>
    public class NoEncryption : IJsonEncrypter
    {
        public bool SaveToDisk(JObject jObject, string filePath, string fileName)
        {
            if (jObject == null)
                return false;

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            string finalPath = Path.Combine(filePath, fileName);
            jObject.ToString(Formatting.Indented).ToJsonFile(finalPath);
            return true;
        }


        public JObject ReadFromDisk(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string fileAsString = File.ReadAllText(filePath);
            return JObject.Parse(fileAsString);
        }
    }
}