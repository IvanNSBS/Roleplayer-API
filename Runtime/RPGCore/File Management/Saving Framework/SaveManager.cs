using System.IO;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPGCore.FileManagement.SavingFramework.Formatters;
using RPGCore.FileManagement.SavingFramework.JsonEncryption;
using RPGCore.FileManagement.SavingFramework.PrefabManager;
using RPGCore.Loggers;
using UnityEditor;

namespace RPGCore.FileManagement.SavingFramework
{
    /// <summary>
    /// Save manager handles Save game Load and Save Events.
    /// </summary>
    [RequireComponent(typeof(DefaultPrefabManager))]
    public class SaveManager : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Hash of Save/Load Events listeners
        /// </summary>
        private Dictionary<string, Saveable> m_subscribersHash;
        
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
        public const string FILE_NAME = "save.dat";
        #endregion Constants
      
        
        #region MonoBehaviour
        private void Awake()
        {
            m_subscribersHash = new Dictionary<string, Saveable>();
            m_subscribersHash = FindObjectsOfType<Saveable>().ToDictionary(x => x.m_componentId, x => x);
            m_jsonEncrypter = new NoEncryption();
            m_formatPolicy = new DefaultFormatter();
            m_prefabManager = GetComponent<IPrefabManager>();
                        
            DontDestroyOnLoad(this);
        }
        #endregion MonoBehaviour
        

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
            //Reaload hash to make sure objects that were spawned later will be considered
            m_subscribersHash = FindObjectsOfType<Saveable>().ToDictionary(x => x.m_componentId, x => x);
            Dictionary<Saveable, JObject> savedComponents = new Dictionary<Saveable, JObject>();

            foreach (var subscriber in m_subscribersHash)
            {
                var tuple = subscriber.Value.SaveComponents();
                savedComponents.Add(tuple.Item1, tuple.Item2);
            }

            JObject saveFileString = m_formatPolicy.Format(savedComponents);
            m_jsonEncrypter.SaveToDisk(saveFileString, SAVE_PATH, FILE_NAME);
        }

        /// <summary>
        /// Load Save Game Event.
        /// Reads and undo binarizing through the BinarizePolicy
        /// and undo the formating through the Formatting Policy
        /// to prepare the json to be loadable from the object
        /// Saveable Component.
        /// </summary>
        public virtual void Load()
        {
            string fullPath = Path.Combine(SAVE_PATH, FILE_NAME);
            JObject saveFileObject = m_jsonEncrypter.ReadFromDisk(fullPath);
            var undo = m_formatPolicy.UndoFormatting(saveFileObject);
            
            Dictionary<string, Saveable> newSubscribersHash = new Dictionary<string, Saveable>();

            foreach (var saveObject in undo)
            {
                // Scene game object is on SaveGame. Load it
                if (m_subscribersHash.ContainsKey(saveObject.Key))
                {
                    m_subscribersHash[saveObject.Key].LoadComponents(saveObject.Value);
                    // Add the updated object to the new subscriber hash and remove it from
                    // the old one
                    newSubscribersHash.Add(saveObject.Key, m_subscribersHash[saveObject.Key]);
                    m_subscribersHash.Remove(saveObject.Key);
                }
                else // Not present in the save game. Try to instantiate it
                {
                    var saveableGameObject = m_prefabManager.InstantiatePrefab(saveObject.Key, saveObject.Value);
                    if (saveableGameObject != null)
                    {
                        Saveable saveableComponent = saveableGameObject.GetComponent<Saveable>();
                        newSubscribersHash.Add(saveableComponent.m_componentId, saveableComponent);
                    }
                }
            }
            
            // All subscribers that are here are not present in the save file, so they must be
            // deleted
            foreach (var remainingSubscriber in m_subscribersHash)
            {
                Destroy(remainingSubscriber.Value.gameObject);
            }
            
            m_subscribersHash = newSubscribersHash;
        }
        #endregion Savegame Events
    }
}