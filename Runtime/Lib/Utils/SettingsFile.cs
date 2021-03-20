using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lib.Utils
{
    public static class SettingsUtils
    {
        public static T GetSettings<T>(string folderPath, string filePath) where T : ScriptableObject
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            var settings = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (settings != null)
                return settings;
            
            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();

            return asset;
        }
    }
}