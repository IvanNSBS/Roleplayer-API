using System;
using System.IO;
using INUlib.Utils;
using UnityEngine;

namespace INUlib.Core.Persistence.Data
{
    public class PersistenceSettings : ScriptableObject
    {
        #region Static Fields
        public static string settingsFileName = "Persistence Settings";
        public static string settingsSubFolderName = "";
        #endregion Static Fields
        
        #region Serializable Fields
        [Header("Save File Data")]
        [SerializeField] private string m_saveSubFolder = "Persistence";
        [SerializeField] private string m_fileName = "save";
        [SerializeField] private string m_fileExtension = ".json";
        
        [Header("Save File Encryption")]
        public EncryptionMode m_encryptionMode;

        /// <summary>
        /// Prefab manager used to instantiate objects that are in save
        /// game but not in scene
        /// </summary>
        [Header("Prefab Settings")] 
        [SerializeField] private string m_prefabFolder = "Assets/Prefabs";
        [Tooltip("This folder is actually a subfolder of Resources. To get the full path, use FullPrefabElementFolder property")]
        [SerializeField] private string m_prefabElementFolder = "Persistence/Prefab Elements";
        #endregion Serializable Fields

        #region Properties
        public string FilePath => Path.Combine(FileFolder, m_fileName + m_fileExtension);
        public string FileFolder =>  String.IsNullOrEmpty(m_saveSubFolder) ? 
            Application.persistentDataPath : Path.Combine(Application.persistentDataPath, m_saveSubFolder);
        public string PrefabFolder => m_prefabFolder;
        public string PrefabElementFolder => m_prefabElementFolder;
        public string FullPrefabElementFolder => Path.Combine("Assets/Resources", m_prefabElementFolder);
        #endregion Properties
        
        
        #region Methods
        public static PersistenceSettings GetPersistenceSettings()
        {
            return SettingsUtils.GetSettings<PersistenceSettings>(settingsSubFolderName, settingsFileName);
        }
        #endregion Methods
    }
}