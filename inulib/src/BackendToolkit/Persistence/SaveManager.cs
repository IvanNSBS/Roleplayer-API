using System;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using INUlib.BackendToolkit.Persistence.Interfaces;
using INUlib.Core;
using System.ComponentModel;

namespace INUlib.BackendToolkit.Persistence
{
    /// <summary>
    /// Save manager handles Save game Load and Save Events.
    /// </summary>
    public class SaveManager
    {
        #region Singleton
        private static SaveManager s_instance;

        public static SaveManager Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SaveManager();

                return s_instance;
            }
        }
        #endregion Singleton
        
        #region Fields
        /// <summary>
        /// Encryption Policy
        /// </summary>
        private JsonEncryption _jsonEncrypter;
        private Dictionary<string, DataStore> _dataStoreHash;
        private PersistenceSettings _settings;
        #endregion Fields
        

        #region Constuctor
        private SaveManager()
        {
            InitializeVariables();
            SetupRegisteredStores();
            DataStoreRegistry.OnStoreRegistered += OnStoreRegistered;
        }

        ~SaveManager()
        {
            DataStoreRegistry.OnStoreRegistered -= OnStoreRegistered;
        }
        #endregion MonoBehaviour Methods
        

        #region Save Game Events
        public void SetSettings(PersistenceSettings settings) => _settings = settings;

        /// <summary>
        /// Serializes every registered data store and store to disk
        /// based on the encryption method an other persistence settings
        /// </summary>
        public void Save()
        {
            JObject saveFileResult = new JObject();
            
            _jsonEncrypter.EncryptionMode = _settings.m_encryptionMode;

            foreach (var dataStore in _dataStoreHash)
                saveFileResult.Add(DataStoreRegistry.TypeToString(dataStore.Value.GetType()), dataStore.Value.SerializeStoredData());
            
            _jsonEncrypter.SaveToDisk(saveFileResult, _settings.FileFolder, _settings.FileName);
        }

        /// <summary>
        /// Load All Registered DataStores cache.
        /// </summary>
        /// <param name="deserializeAfter">Whether the DataStores should be immediately loaded from the save file</param>
        /// <returns>True if save was loaded. False Otherwise</returns>
        public bool LoadStoresCacheFromSaveFile(bool deserializeAfter = false)
        {
            _jsonEncrypter.EncryptionMode = _settings.m_encryptionMode;
            JObject saveFileObject = _jsonEncrypter.ReadFromDisk(_settings.FilePath);

            if (saveFileObject == null)
                return false;
            
            foreach (var dataStore in saveFileObject)
                _dataStoreHash[dataStore.Key].SetCache(dataStore.Value as JObject);
            
            if(deserializeAfter)
                LoadStoresFromCache();
            
            return true;
        }

        /// <summary>
        /// Loads a single Data Store from the save file
        /// </summary>
        /// <param name="deserializeAfter">Whether the DataStore should be immediately loaded from the save file</param>
        /// <typeparam name="T">The Data Store to be loaded</typeparam>
        /// <returns>True if Loaded. False otherwise or if the data store hasn't been registered</returns>
        public bool LoadSingleStoreCacheFromSaveFile<T>(bool deserializeAfter = false) where T : DataStore
        {
            string key = DataStoreRegistry.TypeToString(typeof(T));
            
            _jsonEncrypter.EncryptionMode = _settings.m_encryptionMode;
            JObject saveFileObject = _jsonEncrypter.ReadFromDisk(_settings.FilePath);

            if (saveFileObject == null)
                return false;

            if (saveFileObject.ContainsKey(key))
                _dataStoreHash[key].SetCache(saveFileObject[key] as JObject);

            if (deserializeAfter)
                _dataStoreHash[key].DeserializeStoredObjectsFromCache();
            
            return true;
        }

        /// <summary>
        /// Loads all stores from their current cache data
        /// </summary>
        /// <param name="excludeThose">Array of Stores that shouldn't be loaded</param>
        public void LoadStoresFromCache(params Type[] excludeThose)
        {
            foreach (var hash in _dataStoreHash)
            {
                if (Array.FindIndex(excludeThose, t => hash.Value.GetType() == t) != -1) 
                    continue;

                hash.Value.DeserializeStoredObjectsFromCache();
            }
        }
        #endregion Save Game Events
        
        
        #region Methods
        /// <summary>
        /// Retrieves the Registered Data Store 
        /// </summary>
        /// <param name="considerSubTypes">If should consider child types on the search</param>
        /// <typeparam name="T">The type of the data store</typeparam>
        /// <returns>The DataStore of Type T. Null if the store is not registered</returns>
        public T GetDataStoreFor<T>(bool considerSubTypes = false) where T : DataStore
        {
            if (!considerSubTypes)
            {
                string key = DataStoreRegistry.TypeToString(typeof(T));
                if (_dataStoreHash.ContainsKey(key))
                    return (T) _dataStoreHash[key];
            }
            else
            {
                foreach (var dataStore in _dataStoreHash)
                {
                    if (dataStore.Value.GetType().IsSubclassOf(typeof(T)) || typeof(T) == dataStore.Value.GetType())
                        return (T) dataStore.Value;
                }
            }
            return null;
        }

        public bool DeleteSaveFileFromDisk()
        {
            if (File.Exists(_settings.FilePath))
            {
                File.Delete(_settings.FilePath);
                return true;
            }

            return false;
        }

        // TODO: Integrate SaveManager with 
        /// <summary>
        /// Clears singleton. Mostly used on testing since SaveManager is a singleton
        /// </summary>
        public static void ResetDataStores()
        {
            if (s_instance != null)
            {
                if(s_instance._dataStoreHash != null)
                    s_instance._dataStoreHash = new Dictionary<string, DataStore>();
            }
        }

        #endregion Methods
        
        
        #region Utility Methods
        private void InitializeVariables()
        {
            _jsonEncrypter = new JsonEncryption();
            _dataStoreHash = new Dictionary<string, DataStore>();
            _settings = new PersistenceSettings();
        }
        
        private void SetupRegisteredStores()
        {
            var registeredStores = DataStoreRegistry.GetRegisteredStores();
            if (registeredStores.Count == 0)
                Logger.Warning("There are no registered data stores in Save Manager. Nothing will be saved");

            foreach (var dataStore in registeredStores)
                RegisterType(dataStore.Value);
        }
        
        private void RegisterType(Type type)
        {
            string key = DataStoreRegistry.TypeToString(type);
            if (!_dataStoreHash.ContainsKey(key))
            {
                object[] args = { this };
                var packageObject = (DataStore)Activator.CreateInstance(type, args);
                _dataStoreHash.Add(key, packageObject);
            }
            else
            {
                Logger.Warning("There Are Duplicate Stores on Settings!!");
            }
        }
        
        private void OnStoreRegistered(Type type)
        {
            RegisterType(type);
        }
        #endregion Utility Methods
    }
}