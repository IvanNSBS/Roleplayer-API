using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace INUlib.BackendToolkit.Persistence
{
    public enum EncryptionMode
    {
        NoEncryption,
        Binarize,
    }

    /// <summary>
    /// A Json Encrypter transform json string to another format,
    /// to avoid human-readable formats to avoid exploits and
    /// possibly use less disk space than plain json
    /// </summary>
    public class JsonEncryption
    {
        #region Fields
        private EncryptionMode m_encryptionMode = EncryptionMode.NoEncryption;
        #endregion Fields
        
        #region Properties
        public EncryptionMode EncryptionMode
        {
            get => m_encryptionMode;
            set => m_encryptionMode = value;
        }
        #endregion Properties

        
        #region Encryption Methods
        private string EncryptJsonString(string jsonString)
        {
            switch (m_encryptionMode)
            {
                case EncryptionMode.NoEncryption:
                    return jsonString;
                
                case EncryptionMode.Binarize:
                    var byteArray = Encoding.UTF8.GetBytes(jsonString);
                    var encodedString = Convert.ToBase64String(byteArray);
                    return encodedString;
                
                default:
                    return jsonString;
            }
        }

        private JObject DecryptJsonString(string jsonString)
        {
            switch (m_encryptionMode)
            {
                case EncryptionMode.NoEncryption:
                    return JObject.Parse(jsonString);
                
                case EncryptionMode.Binarize:
                    var decodedStringBytes = Convert.FromBase64String(jsonString);
                    var decodedString = Encoding.UTF8.GetString(decodedStringBytes);;
                    return JObject.Parse(decodedString);
                    
                default:
                    return JObject.Parse(jsonString);
            }
        }
        #endregion Enctyption Methods
        
        
        #region Save File Read & Write Methods

        /// <summary>
        /// Saves a json file to disk using the PersistenceSettings
        /// </summary>
        /// <param name="jObject">Json Object</param>
        /// <param name="settings">Project persistence settings</param>
        /// <returns></returns>
        public bool SaveToDisk(JObject jObject, PersistenceSettings settings)
        {
            if (jObject == null)
                return false;

            if (!Directory.Exists(settings.FileFolder))
                Directory.CreateDirectory(settings.FileFolder);

            string jsonString = jObject.ToString(Formatting.Indented);
            string encryptionResult = EncryptJsonString(jsonString);

            File.WriteAllText(settings.FilePath, encryptionResult);
            return true;
        }

        /// <summary>
        /// Reads a save file from the disk
        /// </summary>
        /// <param name="settings">Project persistence settings</param>
        /// <returns>The save file as JObject json</returns>
        public JObject ReadFromDisk(PersistenceSettings settings)
        {
            if (!File.Exists(settings.FilePath))
                return null;

            string fileAsString = File.ReadAllText(settings.FilePath);
            return DecryptJsonString(fileAsString);
        }
        #endregion Save File Read & Write Methods
    }
}