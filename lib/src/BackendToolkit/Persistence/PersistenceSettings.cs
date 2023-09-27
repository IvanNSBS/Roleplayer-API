using System;
using System.IO;

namespace INUlib.BackendToolkit.Persistence
{
    public class PersistenceSettings
    {
        #region Static Fields
        public static string settingsFileName = "Persistence Settings";
        public static string settingsSubFolderName = "INUlib";
        #endregion Static Fields
        
        #region Serializable Fields
        private string m_saveSubFolder = "Persistence";
        private string m_fileName = "save";
        private string m_fileExtension = ".json";
        public EncryptionMode m_encryptionMode;

        /// <summary>
        /// Prefab manager used to instantiate objects that are in save
        /// game but not in scene
        /// </summary>
        private string m_prefabFolder = "Assets/Prefabs";
        private string m_prefabElementFolder = "Persistence/Prefab Elements";
        #endregion Serializable Fields

        #region Properties
        public string FileName => m_fileName;
        public string FilePath => Path.Combine(FileFolder, m_fileName + m_fileExtension);
        public string FileFolder => Path.Combine(m_saveSubFolder, m_saveSubFolder);
        public string PrefabFolder => m_prefabFolder;
        public string PrefabElementFolder => m_prefabElementFolder;
        public string FullPrefabElementFolder => Path.Combine("Assets/Resources", m_prefabElementFolder);
        #endregion Properties
    }
}