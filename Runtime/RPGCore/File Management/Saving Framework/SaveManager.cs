using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPGCore.Utils.Extensions;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using RPGCore.FileManagement.SavingFramework.Formatters;
using RPGCore.FileManagement.SavingFramework.PrefabManager;
using RPGCore.FileManagement.SavingFramework.JsonEncryption;

namespace RPGCore.FileManagement.SavingFramework
{
    public enum SaveableOptions
    {
        Global,
        Scenes
    }
    
    /// <summary>
    /// Save manager handles Save game Load and Save Events.
    /// </summary>
    [RequireComponent(typeof(DefaultPrefabManager))]
    public class SaveManager : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Hash of Save/Load Events listeners. They are separated
        /// between scenes(using scene build index as the int in the dictionary)
        /// </summary>
        private Dictionary<int, Dictionary<string, Saveable>> m_subscribersHash;

        /// <summary>
        /// Cache of current save game.
        /// </summary>
        private JObject m_saveGameCache;
        
        /// <summary>
        /// Formatting policy used to concatenate all m_subscribers
        /// json representation into a single json file to save to disk
        /// </summary>
        private IJsonFormatter m_formatPolicy;

        /// <summary>
        /// Encryption Policy
        /// </summary>
        private IJsonEncrypter m_jsonEncrypter;

        /// <summary>
        /// Prefab manager used to instantiate objects that are in save
        /// game but not in scene
        /// </summary>
        private IPrefabManager m_prefabManager;
        #endregion Fields
        
        #region Constants
        public const string SAVE_PATH = "D:\\Unity Projects\\Project Small Sandbox\\Assets\\Resources\\Saves";
        public const string FILE_NAME = "save.json";
        #endregion Constants
      
        #region Singleton
        private static SaveManager _instance;
        public static SaveManager Instance => _instance;
        #endregion Singleton
        
        #region Properties
        public Dictionary<int, Dictionary<string, Saveable>> SubscribersHash => m_subscribersHash;
        #endregion Properties
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            if(_instance != this)
                Destroy(gameObject);

            m_saveGameCache = new JObject();
            m_subscribersHash = new Dictionary<int, Dictionary<string, Saveable>>();
            m_jsonEncrypter = new NoEncryption();
            m_formatPolicy = new DefaultFormatter();
            m_prefabManager = GetComponent<IPrefabManager>();

            string fullPath = Path.Combine(SAVE_PATH, FILE_NAME);
            m_saveGameCache = m_jsonEncrypter.ReadFromDisk(fullPath);
        }
        #endregion MonoBehaviour Methods
        

        #region Savegame Events
        /// <summary>
        /// Save Game Event.
        /// Calls the SaveComponents Method for every Saveable
        /// that have been subscribed to the save manager and
        /// unify them through the FormattingPolicy to be a
        /// single json string and Binarize it through the
        /// BinarizePolicy
        /// </summary>
        public virtual void Save()
        {
            Dictionary<Saveable, JObject> savedComponents = new Dictionary<Saveable, JObject>();

            foreach (var scene in m_subscribersHash)
            {
                foreach (var subscriber in scene.Value)
                {
                    var tuple = subscriber.Value.SaveComponents();
                    savedComponents.Add(tuple.Item1, tuple.Item2);
                }
            }

            JObject saveFileJson = m_formatPolicy.Format(savedComponents, m_saveGameCache);
            m_jsonEncrypter.SaveToDisk(saveFileJson, SAVE_PATH, FILE_NAME);
        }

        /// <summary>
        /// Load Save Event.
        /// Reads and undo binarizing through the BinarizePolicy
        /// and undo the formating through the Formatting Policy
        /// to prepare the json to be loadable from the object
        /// Saveable Component.
        /// </summary>
        public virtual void Load()
        {
            string fullPath = Path.Combine(SAVE_PATH, FILE_NAME);
            JObject saveFileObject = m_jsonEncrypter.ReadFromDisk(fullPath);
            var savedGameObjects = m_formatPolicy.UndoFormatting(saveFileObject, m_saveGameCache);
            
            foreach (var savedGameObject in savedGameObjects)
            {
                int currentSceneIndex = savedGameObject.Key;
                LoadScene(currentSceneIndex, savedGameObject.Value);
            }
        }

        public virtual void LoadScene(int sceneIndex, List<SavedObject> savedGameObjects)
        {
            var loadedScenes = SceneUtils.GetLoadedScenesAndBuildIndex();
            Scene activeScene = SceneManager.GetActiveScene();
            var newSubscribersHash = new Dictionary<int, Dictionary<string, Saveable>>();

            foreach (var savedObject in savedGameObjects)
            {
                if (!m_subscribersHash.ContainsKey(sceneIndex))
                    m_subscribersHash.Add(sceneIndex, new Dictionary<string, Saveable>());            
                if(!newSubscribersHash.ContainsKey(sceneIndex))
                    newSubscribersHash.Add(sceneIndex, new Dictionary<string, Saveable>());
                
                if (m_subscribersHash[sceneIndex].ContainsKey(savedObject.id))
                {
                    string objectId = savedObject.id;
                    m_subscribersHash[sceneIndex][objectId].LoadComponents(savedObject.jsonRepresentation);
                    // Add the updated object to the new subscriber hash and remove it from
                    // the old one
                    newSubscribersHash[sceneIndex].Add(objectId, m_subscribersHash[sceneIndex][objectId]);
                    m_subscribersHash[sceneIndex].Remove(objectId);
                }
                else
                {
                    // Scene that contains this object is not open, do not try to instantiate object
                    if (!loadedScenes.ContainsKey(sceneIndex))
                        continue;
                            
                    var saveableGameObject = m_prefabManager.InstantiatePrefab(savedObject.id, savedObject.jsonRepresentation);
                    if (saveableGameObject != null)
                    {
                        Saveable saveableComponent = saveableGameObject.GetComponent<Saveable>();
                        newSubscribersHash[sceneIndex].Add(saveableComponent.ComponentId, saveableComponent);
                        if(sceneIndex == -1)
                            SceneManager.MoveGameObjectToScene(saveableGameObject, activeScene);
                        else
                            SceneManager.MoveGameObjectToScene(saveableGameObject, loadedScenes[sceneIndex]);
                    }
                }
            }
            
            // All subscribers that are still here are not present in the save file,
            // so they must be deleted
            foreach (var remainingSubscriber in m_subscribersHash[sceneIndex])
                Destroy(remainingSubscriber.Value.gameObject);
        }
        #endregion Savegame Events
        
        
        #region Methods
        public void AddSubscriber(Saveable subscriber)
        {
            int sceneIndex = subscriber.m_saveableOptions == SaveableOptions.Global ? -1 : subscriber.gameObject.scene.buildIndex;

            if(!m_subscribersHash.ContainsKey(sceneIndex))
                m_subscribersHash.Add(sceneIndex, new Dictionary<string, Saveable>());
            
            if(!m_subscribersHash[sceneIndex].ContainsKey(subscriber.ComponentId))
                m_subscribersHash[sceneIndex].Add(subscriber.ComponentId, subscriber);
        }

        public void RemoveSubscriber(Saveable subscriber)
        {
            int sceneIndex = subscriber.gameObject.scene.buildIndex;
            
            if (!m_subscribersHash.ContainsKey(sceneIndex))
                return;
            
            if(m_subscribersHash[sceneIndex].ContainsKey(subscriber.ComponentId))
                m_subscribersHash[sceneIndex].Remove(subscriber.ComponentId);
        }
        #endregion Methods
    }
}