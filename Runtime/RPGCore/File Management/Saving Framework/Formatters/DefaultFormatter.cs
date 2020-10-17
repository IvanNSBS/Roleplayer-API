using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    /// <summary>
    /// DefaultFormatter simply puts all objects into a json.
    /// No scene or any other data is considered in the operations
    /// </summary>
    public class DefaultFormatter : IJsonFormatter
    {
        public JObject Format(Dictionary<Saveable, JObject> componentsToSave)
        {
            JObject result = new JObject();
            JObject global = new JObject();
            JObject scene = new JObject();
            foreach (var saveComponent in componentsToSave)
            {
                Saveable saveable = saveComponent.Key;
                JObject saveableJson = saveComponent.Value;
                switch (saveable.m_saveableOptions)
                {
                    case SaveableOptions.Global:
                        global.Add(saveable.m_componentId, saveableJson);
                        break;

                    case SaveableOptions.Scenes:
                        int sceneIndex = saveComponent.Key.gameObject.scene.buildIndex;
                        string sceneKey = sceneIndex+"_Scene_" + saveable.gameObject.scene.name;

                        JObject saveableObject = new JObject();
                        saveableObject.Add(saveable.m_componentId, saveableJson);
                        if (scene.ContainsKey(sceneKey))
                            scene[sceneKey].Value<JObject>().Add(saveable.m_componentId, saveableJson);
                        else
                            scene.Add(sceneKey, saveableObject) ;
                        break;
                    default:
                        global.Add(saveable.m_componentId, saveableJson);
                        break;
                }
            }
            
            result.Add(SaveableOptions.Global.ToString(), global);
            result.Add(SaveableOptions.Scenes.ToString(), scene);

            return result;
        }

        public Dictionary<int, List<SavedObject>> UndoFormatting(JObject saveGameJson)
        {
            Dictionary<int, List<SavedObject>> result = new Dictionary<int, List<SavedObject>>();
            JObject globalObjects = saveGameJson[SaveableOptions.Global.ToString()].Value<JObject>();
            JObject sceneObjects = saveGameJson[SaveableOptions.Scenes.ToString()].Value<JObject>();

            foreach (var obj in globalObjects)
            {
                if (!result.ContainsKey(-1))
                {
                    SavedObject savedObject = new SavedObject(obj.Key, obj.Value as JObject);
                    result.Add(-1, new List<SavedObject>() { savedObject });
                }
                else
                {
                    SavedObject savedObject = new SavedObject(obj.Key, obj.Value as JObject);
                    result[-1].Add(savedObject);
                }
            }

            foreach (var scene in sceneObjects)
            {
                string sceneBuildIndexString = scene.Key.Split('_')[0];
                int sceneBuildIndex = int.Parse(sceneBuildIndexString);
                JObject objects = scene.Value as JObject;
                
                if (objects == null)
                    continue;    
                
                foreach (var obj in objects)
                {
                    if (!result.ContainsKey(sceneBuildIndex))
                    {
                        SavedObject savedObject = new SavedObject(obj.Key, obj.Value as JObject);
                        result.Add(sceneBuildIndex, new List<SavedObject>() { savedObject });
                    }
                    else
                    {
                        SavedObject savedObject = new SavedObject(obj.Key, obj.Value as JObject);
                        result[sceneBuildIndex].Add(savedObject);
                    }
                }
            }
            return result;
        }
    }
}