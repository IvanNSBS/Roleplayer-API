using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPGCore.Persistence.Data;
using RPGCore.Persistence.Interfaces;

namespace RPGCore.Persistence.GameObjects
{
    public class PersistentGameObject : MonoBehaviour, IDataStoreElement<GameObjectStore>
    {
        #region Fields
        /// <summary>
        /// GameObject Unique Identifier
        /// </summary>
        [SerializeField] private string m_componentId;

        /// <summary>
        /// Prefab reference id to instantiate the gameObject through
        /// SaveManager's PrefabManager
        /// </summary>
        public PrefabManagerElement m_prefabManagerElement;
        
        /// <summary>
        /// Dictionary of ISaveableData Name and the ISaveableData object
        /// </summary>
        private Dictionary<string, GameObjectSurrogate> m_saveableComponents;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Getter and Setter for GameObject unique Identifier
        /// </summary>
        public string ObjectId
        {
            get => m_componentId;
            set => m_componentId = value;
        }

        public GameObjectStore DataStore { get; private set; }
        #endregion Properties
        
        
        #region MonoBehaviour Methods
        /// <summary>
        /// Unity Start Event guarantee that the Saveable ID is unique
        /// and add it to the save manager subscriber list
        /// </summary>
        private void Start()
        {
            RegisterToDataStore<GameObjectStore>();
        }

        private void Awake()
        {
            if(m_saveableComponents == null)
                m_saveableComponents = GetComponentsInChildren<GameObjectSurrogate>().
                    ToDictionary(x=>x.GetType().Name + $"_{x.Id}", x => x);
        }

        private void OnDestroy()
        {
            RemoveFromDataStore();
        }
        #endregion MonoBehaviour Methods
    
    
        #region Methods
        public void RegisterToDataStore<U>() where U : GameObjectStore
        {
            var saveManager = SaveManager.Instance;
            if (!saveManager)
            {
                Debug.LogWarning($"Trying to register {gameObject.name} but Save Manager is not present in scene.");
                return;
            }
            var store = saveManager.GetDataStore<U>(true);
            if (store == null)
            {
                Debug.LogWarning($"Trying to register {gameObject.name} to Data Store {typeof(U)} but that store hasn't been registered");
                return;
            }
            
            store.AddGameObject(this);
            DataStore = store;
        }

        public void RemoveFromDataStore()
        {
            DataStore?.RemoveGameObject(this);
        }

        /// <summary>
        /// Loops through every ISaveableData on this gameObject and
        /// calls their Save method to get the surrogate json representation
        /// and unify them in a single json string
        /// </summary>
        /// <returns>Tuple containing this Saveable and the json representation of all the objects</returns>
        public JObject Serialize()
        {
            JObject jsonResult = new JObject();
            jsonResult.Add("prefabId", m_prefabManagerElement.Id);
            foreach (var saveable in m_saveableComponents)
            {
                JObject save = saveable.Value.Save();
                jsonResult.Add(saveable.Key, save);
            }
            
            return jsonResult;
        }

        /// <summary>
        /// Loops through every ISaveableData in this gameObject and
        /// calls their Load from the entire gameObject json representation
        /// </summary>
        /// <param name="componentJson">JObject containing the entire object json representation</param>
        /// <returns>True if all components were loaded. False otherwise</returns>
        public bool Deserialize(JObject componentJson)
        {
            bool result = true;
            // m_prefabReferenceId = componentJson["prefabId"]?.ToString();
            foreach (var saveable in m_saveableComponents)
            {
                if (componentJson.ContainsKey(saveable.Key))
                    saveable.Value.Load(componentJson[saveable.Key] as JObject);
                else
                    result = false;
            }
            return result;
        }
        #endregion Methods
    }
}