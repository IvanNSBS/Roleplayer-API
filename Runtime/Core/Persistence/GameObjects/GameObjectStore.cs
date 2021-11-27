using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using INUlib.Core.Persistence.Interfaces;
using Common.Utils.Extensions;
using UnityEngine.SceneManagement;

namespace INUlib.Core.Persistence.GameObjects
{
    public class GameObjectStore : DataStore
    {
        #region Fields
        /// <summary>
        /// Int represents the scene index
        /// </summary>
        private Dictionary<int, Dictionary<string, PersistentGameObject>> m_listeners;
        #endregion Fields
        
        #region Constructor
        public GameObjectStore(SaveManager saveManager) : base(saveManager)
        {
            m_listeners = new Dictionary<int, Dictionary<String, PersistentGameObject>>();
        }
        #endregion Constructor
        
        
        #region DataStore Methods
        /// <summary>
        /// Adds a GameObject to the dictionary of listeners in the open scenes
        /// </summary>
        /// <param name="levelIndex">Object's level Index</param>
        /// <param name="persistentGameObject">Object to be added to the listeners dictionary</param>
        public void AddGameObject(int levelIndex, PersistentGameObject persistentGameObject)
        {
            string objectId = persistentGameObject.ObjectId;
            
            // if there's no scene in Data Store, add id
            if (!m_listeners.ContainsKey(levelIndex))
                m_listeners.Add(levelIndex, new Dictionary<string, PersistentGameObject>());
            
            // Checks if the objects are different, but have the same ID. Create a new id
            // for the newer object to avoid collision.
            if (m_listeners[levelIndex].ContainsKey(objectId) && m_listeners[levelIndex][objectId] != persistentGameObject)
            {
                // TODO: This check is only necessary because the current GUID generation method is 
                // manual. Remove this after GUID generation is fixed.
                persistentGameObject.ObjectId = persistentGameObject.gameObject.name + "_" + Guid.NewGuid();
                objectId = persistentGameObject.ObjectId;
            }
            
            // Add object to dictionary if it's not there already
            if(!m_listeners[levelIndex].ContainsKey(objectId))    
                m_listeners[levelIndex].Add(persistentGameObject.ObjectId, persistentGameObject);            
        }

        /// <summary>
        /// Removes a gameobject from the scene listener dictionary.
        /// </summary>
        /// <param name="levelIndex">Object's level Index</param>
        /// <param name="persistentGameObject">Object to be removed</param>
        /// <returns>True if it was removed. False otherwise</returns>
        public bool RemoveGameObject(int levelIndex, PersistentGameObject persistentGameObject)
        {
            if (m_listeners.ContainsKey(levelIndex) &&
                m_listeners[levelIndex].ContainsKey(persistentGameObject.ObjectId))
            {
                bool removed = m_listeners[levelIndex].Remove(persistentGameObject.ObjectId);
                if (m_listeners[levelIndex].Count == 0)
                    m_listeners.Remove(levelIndex);
                
                return removed;
            }

            return false;
        }

        public override JObject SerializeStoredData()
        {
            if(dataStoreCache == null)
                dataStoreCache = new JObject();

            foreach (var scene in m_listeners)
            {
                string sceneKey = scene.Key.ToString();
                
                if(!dataStoreCache.ContainsKey(sceneKey))
                    dataStoreCache[sceneKey] = new JObject();
                    
                foreach (var gameObject in scene.Value)
                {
                    dataStoreCache[sceneKey][gameObject.Key] = gameObject.Value.ToJson();
                }
            }

            return dataStoreCache;
        }

        public override bool DeserializeStoredObjectsFromCache()
        {
            LoadOpenScenes();
            return true;
        }

        public override void ClearSaveStore()
        {
            dataStoreCache = new JObject();
            m_listeners.Clear();
        }

        public override void RemoveStoredObjects() => m_listeners.Clear();
        #endregion DataStore Methods
        
        
        #region Methods
        /// <summary>
        /// Function to load a scene, given it's buildIndex.
        /// Will deserialize the ones present in scene and in save file,
        /// Instantiate and deserialize the ones present in save file but not in scene
        /// And destroy the ones that aren't present in save file.
        /// </summary>
        /// <param name="levelIndex"></param>
        public bool LoadScene(int levelIndex, bool levelIndexIsSceneIndex = false)
        {
            var loadedScenes = SceneUtils.GetLoadedScenesAndBuildIndex();
            Scene targetScene = levelIndexIsSceneIndex ? loadedScenes[levelIndex] : SceneManager.GetActiveScene();

            JObject sceneSavedCache = null;
            if(dataStoreCache.ContainsKey(levelIndex.ToString()))
                sceneSavedCache = dataStoreCache[levelIndex.ToString()] as JObject;
            
            var sceneObjects = m_listeners[levelIndex];
            Dictionary<string, PersistentGameObject> newSceneObjects = new Dictionary<string, PersistentGameObject>();
            
            // If there were objects in the save file cache, load them
            if (sceneSavedCache != null)
            {
                foreach (var savedObject in sceneSavedCache)
                {
                    string objectId = savedObject.Key;
                    JObject objectJsonRepresentation = savedObject.Value as JObject;
                    
                    // if object is present in scene, deserialize it
                    if (sceneObjects.ContainsKey(objectId))
                    {
                        sceneObjects[objectId].FromJson(objectJsonRepresentation);
                        newSceneObjects.Add(objectId, sceneObjects[objectId]);
                    }
                    // if not present in scene, instantiate and load it
                    else
                    {
                        var persistentGO = saveManager.PrefabManager.InstantiatePrefab(objectId, objectJsonRepresentation);
                        if (persistentGO != null)
                        {
                            PersistentGameObject persistentGameObjectComponent = persistentGO.GetComponent<PersistentGameObject>();
                            AddGameObject(levelIndex, persistentGameObjectComponent);
                            SceneManager.MoveGameObjectToScene(persistentGO, targetScene);
                            
                            newSceneObjects.Add(persistentGameObjectComponent.ObjectId, persistentGameObjectComponent);
                        }
                    }

                    sceneObjects.Remove(objectId);
                }
                
                
                // Objects left are not present in save file, so they must be deleted from scene
                // and removed from save file
                foreach (var sceneObject in sceneObjects)
                {
                    GameObject.Destroy(sceneObject.Value.gameObject);
                }

                m_listeners[levelIndex] = newSceneObjects;
                return true;

            }

            return false;
        }
        #endregion Methods
        
        
        #region Utility Methods
        /// <summary>
        /// Function to load the saved data in all open scenes 
        /// </summary>
        public void LoadOpenScenes()
        {
            int[] loadedSceneIndexes = SceneUtils.GetLoadedScenesByBuildIndex();
            foreach (var sceneIndex in loadedSceneIndexes)
            {
                LoadScene(sceneIndex);
            }
        }
        #endregion Utility Methods
    }
}