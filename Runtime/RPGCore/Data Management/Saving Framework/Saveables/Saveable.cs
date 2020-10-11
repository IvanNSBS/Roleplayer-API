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
        /// <summary>
        /// GameObject Unique Identifier
        /// </summary>
        public string m_componentId;
        
        /// <summary>
        /// Dictionary of ISaveableData Name and the ISaveableData object
        /// </summary>
        private Dictionary<string, ISaveableData> m_saveableComponents;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Getter for GameObject unique Identifier
        /// </summary>
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

        /// <summary>
        /// Loops through every ISaveableData on this gameObject and
        /// calls their Save method to get the surrogate json representation
        /// and unify them in a single json string
        /// </summary>
        /// <returns>Tuple containing this Saveable and the json representation of all the objects</returns>
        public Tuple<Saveable, JObject> SaveComponents()
        {
            JObject jsonResult = new JObject();
            foreach (var saveable in m_saveableComponents)
            {
                JObject save = saveable.Value.Save();
                jsonResult.Add(saveable.Key, save);
            }
            
            return new Tuple<Saveable, JObject>(this, jsonResult);
        }

        /// <summary>
        /// Loops through every ISaveableData in this gameObject and
        /// calls their Load from the entire gameObject json representation
        /// </summary>
        /// <param name="componentJson">JObject containing the entire object json representation</param>
        /// <returns>True if all components were loaded. False otherwise</returns>
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