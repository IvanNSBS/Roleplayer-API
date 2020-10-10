using System;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework
{
    [ExecuteInEditMode]
    public class Saveable : MonoBehaviour
    {
        #region Fields
        public string m_componentId;
        private Dictionary<string, ISaveableData> m_saveableComponents;
        #endregion Fields

        #region Properties
        public string ComponentId => m_componentId;
        #endregion Properties
        
        
        #region MonoBehaviour Methods

        private void OnEnable()
        {
            if (String.IsNullOrEmpty(m_componentId))
                m_componentId = gameObject.name + "_" +Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            m_saveableComponents = GetComponents<ISaveableData>().ToDictionary(x=>x.SurrogateName, x => x);
            SaveManager.Instance.AddSubscriber(this);
        }

        private void OnDestroy()
        {
            SaveManager.Instance.RemoveSubscriber(this);
        }
        #endregion MonoBehaviour Methods
    
    
        #region Methods

        public Tuple<Saveable, JObject> SaveComponents()
        {
            JObject jsonResult = new JObject();

            foreach (var saveable in m_saveableComponents)
            {
                string save = saveable.Value.Save();
                jsonResult.Add(saveable.Key, JObject.Parse(save));
            }
            
            return new Tuple<Saveable, JObject>(this, jsonResult);
        }

        public bool LoadComponents(JObject componentJson)
        {
            bool result = true;
            foreach (var saveable in m_saveableComponents)
            {
                result &= saveable.Value.Load(componentJson[saveable.Key] as JObject);
            }
            return result;
        }
        #endregion Methods
    }
}