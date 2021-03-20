using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Essentials.Persistence.Interfaces;
using Lib.Utils.Extensions;
using UnityEngine.SceneManagement;

namespace Essentials.Persistence.GameObjects
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
            Debug.Log("Created a GameObject Store!");
            m_listeners = new Dictionary<int, Dictionary<String, PersistentGameObject>>();
        }
        #endregion Constructor
        
        
        #region DataStore Methods
        /// <summary>
        /// Adds a GameObject to the dictionary of listeners in the open scenes
        /// </summary>
        /// <param name="persistentGameObject">Object to be added to the listeners dictionary</param>
        public void AddGameObject(PersistentGameObject persistentGameObject)
        {
            int sceneIndex = persistentGameObject.gameObject.scene.buildIndex;
            string objectId = persistentGameObject.ObjectId;
            
            // if there's no scene in Data Store, add id
            if (!m_listeners.ContainsKey(sceneIndex))
                m_listeners.Add(sceneIndex, new Dictionary<string, PersistentGameObject>());
            
            // Checks if the objects are different, but have the same ID. Create a new id
            // for the newer object to avoid collision.
            if (m_listeners[sceneIndex].ContainsKey(objectId) && m_listeners[sceneIndex][objectId] != persistentGameObject)
            {
                // TODO: This check is only necessary because the current GUID generation method is 
                // manual. Remove this after GUID generation is fixed.
                persistentGameObject.ObjectId = persistentGameObject.gameObject.name + "_" + Guid.NewGuid();
                objectId = persistentGameObject.ObjectId;
            }
            
            // Add object to dictionary if it's not there already
            if(!m_listeners[sceneIndex].ContainsKey(objectId))    
                m_listeners[sceneIndex].Add(persistentGameObject.ObjectId, persistentGameObject);            
        }

        /// <summary>
        /// Removes a gameobject from the scene listener dictionary.
        /// </summary>
        /// <param name="persistentGameObject">Object to be removed</param>
        /// <returns>True if it was removed. False otherwise</returns>
        public bool RemoveGameObject(PersistentGameObject persistentGameObject)
        {
            int sceneIndex = persistentGameObject.gameObject.scene.buildIndex;
            if (m_listeners.ContainsKey(sceneIndex) &&
                m_listeners[sceneIndex].ContainsKey(persistentGameObject.ObjectId))
            {
                bool removed = m_listeners[sceneIndex].Remove(persistentGameObject.ObjectId);
                if (m_listeners[sceneIndex].Count == 0)
                    m_listeners.Remove(sceneIndex);
                
                return removed;
            }

            return false;
        }

        public override JObject Serialize()
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
                    dataStoreCache[sceneKey][gameObject.Key] = gameObject.Value.Serialize();
                }
            }

            return dataStoreCache;
        }

        public override bool Deserialize(JObject objectJson)
        {
            dataStoreCache = objectJson;
            
            LoadOpenScenes();
            return true;
        }
        #endregion DataStore Methods
        
        
        #region Methods
        /// <summary>
        /// Function to load a scene, given it's buildIndex.
        /// Will deserialize the ones present in scene and in save file,
        /// Instantiate and deserialize the ones present in save file but not in scene
        /// And destroy the ones that aren't present in save file.
        /// </summary>
        /// <param name="sceneIndex"></param>
        public bool LoadScene(int sceneIndex)
        {
            var loadedScenes = SceneUtils.GetLoadedScenesAndBuildIndex();
            Scene targetScene = loadedScenes[sceneIndex];

            JObject sceneSavedCache = null;
            if(dataStoreCache.ContainsKey(sceneIndex.ToString()))
                sceneSavedCache = dataStoreCache[sceneIndex.ToString()] as JObject;
            
            var sceneObjects = m_listeners[sceneIndex];
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
                        sceneObjects[objectId].Deserialize(objectJsonRepresentation);
                        newSceneObjects.Add(objectId, sceneObjects[objectId]);
                    }
                    // if not present in scene, instantiate and load it
                    else
                    {
                        var saveableGameObject = saveManager.PrefabManager.InstantiatePrefab(objectId, objectJsonRepresentation);
                        if (saveableGameObject != null)
                        {
                            PersistentGameObject persistentGameObjectComponent = saveableGameObject.GetComponent<PersistentGameObject>();
                            AddGameObject(persistentGameObjectComponent);
                            SceneManager.MoveGameObjectToScene(saveableGameObject, targetScene);
                            
                            newSceneObjects.Add(persistentGameObjectComponent.ObjectId, persistentGameObjectComponent);
                        }
                    }

                    sceneObjects.Remove(objectId);
                }
            }

            // Objects left are not present in save file, so they must be deleted from scene
            // and removed from save file
            foreach (var sceneObject in sceneObjects)
            {
                GameObject.Destroy(sceneObject.Value.gameObject);
            }

            m_listeners[sceneIndex] = newSceneObjects;
            
            return true;
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