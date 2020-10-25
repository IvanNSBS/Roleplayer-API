using System;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework
{
    public class Saveable : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// GameObject Unique Identifier
        /// </summary>
        [SerializeField] private string m_componentId;

        /// <summary>
        /// Saving Options
        /// </summary>
        public SaveableOptions m_saveableOptions;
        
        /// <summary>
        /// Prefab referenc id to instantiate the gameObject through
        /// SaveManager's PrefabManager
        /// </summary>
        public string m_prefabReferenceId;
        
        /// <summary>
        /// Dictionary of ISaveableData Name and the ISaveableData object
        /// </summary>
        private Dictionary<string, ISaveableData> m_saveableComponents;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Getter and Setter for GameObject unique Identifier
        /// </summary>
        public string ComponentId
        {
            get => m_componentId;
            set => m_componentId = value;
        }

        /// <summary>
        /// Getter for Gameobject Saveable Components
        /// </summary>
        public Dictionary<string, ISaveableData> SaveableComponents => m_saveableComponents; 
        #endregion Properties
        
        
        #region MonoBehaviour Methods
        /// <summary>
        /// Unity Start Event guarantee that the Saveable ID is unique
        /// and add it to the save manager subscriber list
        /// </summary>
        private void Start()
        {
            Debug.Log("Starting...");
            SaveManager.Instance.AddSubscriber(this);
        }

        private void Awake()
        {
            if(m_saveableComponents == null)
                m_saveableComponents = GetComponentsInChildren<ISaveableData>().ToDictionary(x=>x.GetType().Name, x => x);
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
            jsonResult.Add("prefabId", m_prefabReferenceId);
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