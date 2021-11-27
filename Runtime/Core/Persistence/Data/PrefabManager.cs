using UnityEngine;
using INUlib.Common.Debugging.Loggers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using INUlib.Core.Persistence.GameObjects;

#pragma warning disable CS0649
namespace INUlib.Core.Persistence.Data
{
    public class PrefabManager
    {
        #region Fields
        private Dictionary<string, GameObject> m_prefabHash; 
        #endregion Fields
        
        
        #region Constructor
        public PrefabManager(PersistenceSettings settings)
        {
            if (Directory.Exists(settings.FullPrefabElementFolder))
            {
                var prefabElements = Resources.LoadAll<PrefabManagerElement>(settings.PrefabElementFolder);
                m_prefabHash = prefabElements.ToDictionary(p => p.Id, g => g.GameObject);
            }
        }
        #endregion Constructor
        
        
        #region Methods        
        public GameObject InstantiatePrefab(string jsonKey, JObject objectJson)
        {
            string prefabIdName = objectJson["prefabId"].ToString();
            if (!m_prefabHash.ContainsKey(prefabIdName))
            {
                // TODO: Enable again after fixing loggers
                // Loggers.Logger.Instance.Log(LogLevels.Error, "Prefab ID is not valid or prefab ID was not saved as prefabId");
                return null;
            }
            
            var saveable = GameObject.Instantiate(m_prefabHash[prefabIdName]).GetComponent<PersistentGameObject>();
            saveable.gameObject.name = jsonKey;
            saveable.ObjectId = jsonKey;
            saveable.FromJson(objectJson);

            return saveable.gameObject;
        }
        #endregion Methods
    }
}