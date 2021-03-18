using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using RPGCore.Utils;

namespace RPGCore.Persistence.Data
{
    public class PersistenceSettings : ScriptableObject
    {
        #region Static Fields
        public static string settingsFilePath = Path.Combine(Path.Combine("Assets", "Resources"), "Persistence Settings.asset");
        public static string settingsFolderPath = Path.Combine("Assets", "Resources");
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

        [Header("Data Stores")] 
        [SerializeField] private List<DataStoreSelector> m_registeredDataStores;
        #endregion Serializable Fields

        #region Properties
        public List<DataStoreSelector> RegisteredDataStores
        {
            get => m_registeredDataStores;
            set => m_registeredDataStores = value;
        }
        public string FilePath => Path.Combine(FileFolder, m_fileName + m_fileExtension);
        public string FileFolder =>  String.IsNullOrEmpty(m_saveSubFolder) ? 
            Application.persistentDataPath : Path.Combine(Application.persistentDataPath, m_saveSubFolder);
        public string PrefabFolder => m_prefabFolder;
        public string PrefabElementFolder => m_prefabElementFolder;
        public string FullPrefabElementFolder => Path.Combine("Assets/Resources", m_prefabElementFolder);
        #endregion Properties
        
        
        #region Methods
        // TODO: Use global API Name 
        [MenuItem("RPG-API/Persistence/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            var settings = GetPersistenceSettings();
            ProcessStartInfo startInformation = new ProcessStartInfo();
            
            if(!Directory.Exists(settings.FileFolder))
                Directory.CreateDirectory(settings.FileFolder);
            
            startInformation.FileName = settings.FileFolder;
            Process.Start(startInformation);
        }

        [MenuItem("RPG-API/Persistence/Clear Save File")]
        public static void ClearSaveFile()
        {
            var settings = GetPersistenceSettings();
            if(File.Exists(settings.FilePath))
                File.Delete(settings.FilePath);
        }
        
        public static PersistenceSettings GetPersistenceSettings()
        {
            return SettingsUtils.GetSettings<PersistenceSettings>(settingsFolderPath, settingsFilePath);
        }
        #endregion Methods
    }
}