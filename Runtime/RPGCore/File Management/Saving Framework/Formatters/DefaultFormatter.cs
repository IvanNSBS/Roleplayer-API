using Newtonsoft.Json.Linq;
using RPGCore.Utils.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    /// <summary>
    /// DefaultFormatter simply puts all objects into a json.
    /// No scene or any other data is considered in the operations
    /// </summary>
    public class DefaultFormatter : IJsonFormatter
    {
        public JObject Format(Dictionary<Saveable, JObject> componentsToSave, JObject saveGameCache)
        {
            JObject result = new JObject();
            JObject global = new JObject();
            JObject scene = new JObject();

            if (saveGameCache != null && saveGameCache.ContainsKey(SaveableOptions.Global.ToString()))
            {
                global = saveGameCache[SaveableOptions.Global.ToString()].Value<JObject>();
                scene = saveGameCache[SaveableOptions.Scenes.ToString()].Value<JObject>();
            }
            
            foreach (var saveComponent in componentsToSave)
            {
                Saveable saveable = saveComponent.Key;
                JObject saveableJson = saveComponent.Value;
                switch (saveable.m_saveableOptions)
                {
                    case SaveableOptions.Global:
                        global.AddOrUpdate(saveable.m_componentId, saveableJson);
                        break;

                    case SaveableOptions.Scenes:
                        int sceneIndex = saveComponent.Key.gameObject.scene.buildIndex;
                        string sceneKey = sceneIndex+"_Scene_" + saveable.gameObject.scene.name;

                        JObject saveableObject = new JObject();
                        saveableObject.AddOrUpdate(saveable.m_componentId, saveableJson);
                        if (scene.ContainsKey(sceneKey))
                            scene[sceneKey].Value<JObject>().AddOrUpdate(saveable.m_componentId, saveableJson);
                        else
                            scene.Add(sceneKey, saveableObject);
                        break;
                    default:
                        global.AddOrUpdate(saveable.m_componentId, saveableJson);
                        break;
                }
            }
            
            result.Add(SaveableOptions.Global.ToString(), global);
            result.Add(SaveableOptions.Scenes.ToString(), scene);
            saveGameCache = result;
            
            return result;
        }

        public Dictionary<int, List<SavedObject>> UndoFormatting(JObject saveGameJson, JObject savegameCache)
        {
            Dictionary<int, List<SavedObject>> result = new Dictionary<int, List<SavedObject>>();
            JObject globalObjects = saveGameJson[SaveableOptions.Global.ToString()].Value<JObject>();
            JObject sceneObjects = saveGameJson[SaveableOptions.Scenes.ToString()].Value<JObject>();
            savegameCache = saveGameJson;
            
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