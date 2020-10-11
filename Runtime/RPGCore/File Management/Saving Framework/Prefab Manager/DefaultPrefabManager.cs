using System;
using UnityEngine;
using RPGCore.Loggers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

#pragma warning disable CS0649
namespace RPGCore.FileManagement.SavingFramework.PrefabManager
{
    public class DefaultPrefabManager : MonoBehaviour, IPrefabManager
    {
        #region GameTuple Class
        [Serializable]
        private class GameTuple
        {
            public string id;
            public GameObject gameObject;
        }        
        #endregion GameTuple class
        
        #region Fields
        [SerializeField] private List<GameTuple> prefabs;
        private Dictionary<string, GameObject> m_prefabs; 
        #endregion Fields
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            m_prefabs = new Dictionary<string, GameObject>();
            foreach (var prefab in prefabs)
            {
                m_prefabs.Add(prefab.id, prefab.gameObject);
            }
        }
        #endregion MonoBehaviour Methods
        
        
        #region IPrefabManager Methods
        public GameObject InstantiatePrefab(string jsonKey, JObject objectJson)
        {
            string prefabIdName = objectJson["prefabId"].ToString();
            if (!m_prefabs.ContainsKey(prefabIdName))
            {
                Loggers.Logger.Instance.Log(LogLevels.Error, "Prefab ID is not valid or prefab ID was not saved as prefabId");
                return null;
            }
            
            var saveable = Instantiate(m_prefabs[prefabIdName]).GetComponent<Saveable>();
            saveable.gameObject.name = jsonKey;
            saveable.m_componentId = jsonKey;
            saveable.LoadComponents(objectJson);

            return saveable.gameObject;
        }
        #endregion IPrefabManager Methods
    }
}