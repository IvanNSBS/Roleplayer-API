using System.IO;
using UnityEngine;

namespace Lib.Utils
{
    public static class SettingsUtils
    {
        public static T GetSettings<T>(string resourcesSubFolder, string fileName) where T : ScriptableObject
        {
            string filePath = fileName;
            if (!string.IsNullOrEmpty(resourcesSubFolder))
                filePath = Path.Combine(resourcesSubFolder, fileName);
            
            var settings = Resources.Load<T>(filePath);
            if (settings != null)
                return settings;
            
            T asset = ScriptableObject.CreateInstance<T>();

            #if UNITY_EDITOR
            string folderPath = "Assets\\Resources";
            if (!string.IsNullOrEmpty(resourcesSubFolder))
                folderPath = Path.Combine(folderPath, resourcesSubFolder);
            
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            UnityEditor.AssetDatabase.CreateAsset(asset, Path.Combine(folderPath, fileName) + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            #endif

            return asset;
        }
    }
}