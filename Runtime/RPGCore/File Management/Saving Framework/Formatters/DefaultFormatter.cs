using Newtonsoft.Json.Linq;
using RPGCore.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    /// <summary>
    /// DefaultFormatter simply puts all objects into a json.
    /// No scene or any other data is considered in the operations
    /// </summary>
    public class DefaultFormatter : IJsonFormatter
    {
        #region Fields
        private JObject saveCache;
        #endregion Fields

        #region JsonFormatter Properties
        public JObject SaveGameCache { get => saveCache; set => saveCache = value; }
        #endregion JsonFormatter Properties

        
        #region Constructor

        public DefaultFormatter()
        {
            saveCache = new JObject();
            saveCache.Add(SaveableOptions.Object.ToString(), new JObject());
            saveCache.Add(SaveableOptions.GameObject.ToString(), new JObject());
        }
        #endregion Constructor
        
        
        #region JsonFormatter Methods
        public JObject Format(Dictionary<Saveable, JObject> componentsToSave)
        {
            foreach (var saveComponent in componentsToSave)
            {
                Saveable saveable = saveComponent.Key;
                switch (saveable.m_saveableOptions)
                {
                    case SaveableOptions.Object:
                        // TODO: Add C# Objects handling
                        // SaveObject(saveComponent.Key, saveComponent.Value);
                        break;

                    case SaveableOptions.GameObject:
                        SaveGameObject(saveComponent.Key, saveComponent.Value);
                        break;
                    default:
                        SaveGameObject(saveComponent.Key, saveComponent.Value);
                        break;
                }
            }
            
            return saveCache;
        }
        
        public List<SavedObject> GetSceneSavedGameObjects(int sceneBuildIndex)
        {
            List<SavedObject> result = new List<SavedObject>();
            JObject gameObjects = saveCache[SaveableOptions.GameObject.ToString()].Value<JObject>();

            if (gameObjects.ContainsKey(sceneBuildIndex.ToString()))
            {
                foreach (var gameObject in gameObjects[sceneBuildIndex.ToString()].Value<JObject>())
                {
                    SavedObject savedObject = new SavedObject(gameObject.Key, gameObject.Value as JObject);
                    result.Add(savedObject);
                }
            }

            return result;
        }
        #endregion JsonFormatter Methods
        
        
        #region Auxiliar Methods
        public void SaveGameObject(Saveable saveable, JObject saveableJson)
        {
            string sceneIndex = saveable.gameObject.scene.buildIndex.ToString();
            string gameObject = SaveableOptions.GameObject.ToString();
            
            saveCache[gameObject].TryAdd(sceneIndex, new JObject());
            saveCache[gameObject][sceneIndex].AddOrUpdate(saveable.ComponentId, saveableJson);
        }

        public bool RemoveGameObject(Saveable saveable)
        {
            string gameObject = SaveableOptions.GameObject.ToString();
            string sceneIndex = saveable.gameObject.scene.buildIndex.ToString();

            Debug.Log($"Trying to remove at <<{sceneIndex}>>...");
            Debug.Log(saveCache);

            if (saveCache[gameObject].Value<JObject>().ContainsKey(sceneIndex))
            {
                Debug.Log($"Scene index <<{sceneIndex}>> exists!");
                if (saveCache[gameObject][sceneIndex].Value<JObject>().ContainsKey(saveable.ComponentId))
                {
                    saveCache[gameObject][sceneIndex][saveable.ComponentId].Value<JObject>().Parent.Remove();
                    Debug.Log("Removed object!");
                }
            }
            return false;
        }
        
        public JObject SaveObject(Saveable saveable, JObject saveableJson)
        {
            return null;
            // objects.AddOrUpdate(saveable.ComponentId, saveableJson);
        }

        public bool RemoveObject()
        {
            return false;
        }
        #endregion Auxiliar Methods
    }
}