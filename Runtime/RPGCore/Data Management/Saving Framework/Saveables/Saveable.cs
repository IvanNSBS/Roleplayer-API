using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RPGCore.FileManagement.SavingFramework
{
    [ExecuteInEditMode]
    public class Saveable : MonoBehaviour
    {
        #region Fields
        public string m_componentId;
        private List<ISaveableData> m_saveableComponents;
        #endregion Fields

        #region Properties
        #endregion Properties
        
        
        #region MonoBehaviour Methods

        private void OnEnable()
        {
            if (String.IsNullOrEmpty(m_componentId))
                m_componentId = gameObject.name + "_" +Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            m_saveableComponents = GetComponents<ISaveableData>().ToList();
            SaveManager.Instance.AddSubscriber(this);
        }

        private void OnDestroy()
        {
            SaveManager.Instance.RemoveSubscriber(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveComponents();
            }
            
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadComponents(null);
            }
        }

        #endregion MonoBehaviour Methods
    
    
        #region Methods

        public List<string> SaveComponents()
        {
            List<string> jsonStringCollection = new List<string>();
            JObject jsonResult = new JObject();
            
            foreach (var saveable in m_saveableComponents)
            {
                string save = saveable.Save();
                jsonResult.Add(saveable.SurrogateName, JObject.Parse(save));
                jsonStringCollection.Add(save);    
            }
            
            Debug.Log(jsonResult);
            
            return jsonStringCollection;
        }

        public bool LoadComponents(List<string> jsonStringCollection)
        {
            bool result = true;
            foreach (var saveable in m_saveableComponents)
            {
                result &= saveable.Load("");
            }
            return result;
        }
        #endregion Methods
    }
}