using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Essentials.Persistence.Data;
using Essentials.Persistence.Interfaces;

namespace Essentials.Persistence
{
    /// <summary>
    /// Save manager handles Save game Load and Save Events.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        #region Singleton
        private static SaveManager s_instance;
        public static SaveManager Instance => s_instance;
        #endregion Singleton
        
        #region Fields
        /// <summary>
        /// Encryption Policy
        /// </summary>
        private JsonEncryption m_jsonEncrypter;
        private Dictionary<string, DataStore> m_dataStoreHash;
        private PrefabManager m_prefabManager; 
        private PersistenceSettings m_settings;
        #endregion Fields
        
        #region Properties
        public PrefabManager PrefabManager => m_prefabManager;
        #endregion Properties

        #region Events
        public delegate void PreLoadEvent();
        public delegate void PostLoadEvent(JObject dataStoreJson);
        private event PreLoadEvent PreLoad;
        private event PostLoadEvent PostLoad;
        #endregion Events


        #region MonoBehaviour Methods
        private void Awake()
        {
            if (s_instance == null)
                s_instance = this;
            
            if(s_instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(this);
            
            m_jsonEncrypter = new JsonEncryption();
            m_dataStoreHash = new Dictionary<string, DataStore>();
            m_settings = PersistenceSettings.GetPersistenceSettings();
            m_prefabManager = new PrefabManager(m_settings);


            var registeredStores = DataStoreRegistry.GetRegisteredStores();
            if(registeredStores.Count == 0)
                Debug.LogWarning("There are no registered data stores in Save Manager. Nothing will be saved");
            
            foreach (var dataStore in registeredStores)
                RegisterType(dataStore.Value);
        }

        private void OnEnable()
        {
            DataStoreRegistry.OnStoreRegistered += OnStoreRegistered;
        }

        private void OnDisable()
        {
            DataStoreRegistry.OnStoreRegistered -= OnStoreRegistered;
        }
        #endregion MonoBehaviour Methods
        

        #region Save Game Events
        /// <summary>
        /// Save Game Event.
        /// Calls the SaveComponents Method for every Saveable
        /// that have been subscribed to the save manager and
        /// unify them through the FormattingPolicy to be a
        /// single json string and Binarize it through the
        /// BinarizePolicy
        /// </summary>
        public void Save()
        {
            JObject saveFileResult = new JObject();
            
            m_jsonEncrypter.EncryptionMode = m_settings.m_encryptionMode;

            foreach (var dataStore in m_dataStoreHash)
                saveFileResult.Add(DataStoreRegistry.TypeToString(dataStore.Value.GetType()), dataStore.Value.Serialize());
            
            m_jsonEncrypter.SaveToDisk(saveFileResult, m_settings);
        }

        /// <summary>
        /// Load Save Event.
        /// Reads and undo binarizing through the BinarizePolicy
        /// and undo the formating through the Formatting Policy
        /// to prepare the json to be loadable from the object
        /// Saveable Component.
        /// </summary>
        /// <returns>True if save was loaded. False Otherwise</returns>
        public bool Load()
        {
            PreLoad?.Invoke();

            m_jsonEncrypter.EncryptionMode = m_settings.m_encryptionMode;
            JObject saveFileObject = m_jsonEncrypter.ReadFromDisk(m_settings);

            if (saveFileObject == null)
                return false;
            
            foreach (var dataStore in saveFileObject)
                m_dataStoreHash[dataStore.Key].Deserialize(dataStore.Value as JObject);
            
            PostLoad?.Invoke(saveFileObject);
            return true;
        }
        #endregion Save Game Events
        
        
        #region Methods
        public void AddPreLoadEvent(PreLoadEvent onPreLoad)
        {
            PreLoad += onPreLoad;
        }
        
        public void AddPostLoadEvent(PostLoadEvent onPostLoad)
        {
            PostLoad += onPostLoad;
        }
        
        public void RemovePreLoadEvent(PreLoadEvent onPreLoad)
        {
            PreLoad -= onPreLoad;
        }
        
        public void RemovePostLoadEvent(PostLoadEvent onPostLoad)
        {
            PostLoad -= onPostLoad;
        }
        
        public T GetDataStore<T>(bool considerSubTypes = false) where T : DataStore
        {
            if (!considerSubTypes)
            {
                string key = DataStoreRegistry.TypeToString(typeof(T));
                if (m_dataStoreHash.ContainsKey(key))
                    return (T) m_dataStoreHash[key];
            }
            else
            {
                foreach (var dataStore in m_dataStoreHash)
                {
                    if (dataStore.Value.GetType().IsSubclassOf(typeof(T)) || typeof(T) == dataStore.Value.GetType())
                        return (T)dataStore.Value;
                }
            }
            return null;
        }
        #endregion Methods
        
        
        #region Utility Methods

        private void RegisterType(Type type)
        {
            string key = DataStoreRegistry.TypeToString(type);
            if (!m_dataStoreHash.ContainsKey(key))
            {
                object[] args = { Instance };
                var packageObject = (DataStore)Activator.CreateInstance(type, args);
                m_dataStoreHash.Add(key, packageObject);
            }
            else
            {
                Debug.LogWarning("There Are Duplicate Stores on Settings!!");
            }
        }
        
        private void OnStoreRegistered(Type type)
        {
            RegisterType(type);
        }
        #endregion Utility Methods
    }
}