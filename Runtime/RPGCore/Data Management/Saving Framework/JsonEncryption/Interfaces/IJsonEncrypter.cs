using Newtonsoft.Json.Linq;

namespace RPGCore.FileManagement.SavingFramework.JsonEncryption
{
    /// <summary>
    /// A Json Encrypter transform json string to another format,
    /// to avoid human-readable formats to avoid exploits and
    /// possibly use less disk space than plain json
    /// </summary>
    public interface IJsonEncrypter
    {
        /// <summary>
        /// Saves a JObject to disk at a given path
        /// </summary>
        /// <param name="jObject">The JSON object to save to disk</param>
        /// <param name="filePath">The path to save json to</param>
        /// <param name="fileName">File name aswell as file extension</param>
        /// <returns>True if the file was successfully saved. False otherwise</returns>
        bool SaveToDisk(JObject jObject, string filePath, string fileName);
        
        /// <summary>
        /// Reads a save file from disk
        /// </summary>
        /// <param name="filePath">Path to the save game file</param>
        /// <returns>JObject that represents the save game data as json. False if path was invalid</returns>
        JObject ReadFromDisk(string filePath);
    }
}