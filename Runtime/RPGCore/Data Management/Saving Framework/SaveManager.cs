using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPGCore.DataManagement.Serializers;
using RPGCore.FileManagement.SavingFramework.Formatters;

namespace RPGCore.FileManagement.SavingFramework
{
    public class SaveManager
    {
        #region Fields
        private List<Saveable> m_subscribers;
        private JObject m_currentSaveCache;
        private readonly IJsonFormatter m_formatPolicy;
        #endregion Fields
        
        #region Singleton
        private static SaveManager _instance;
        public static SaveManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new SaveManager();

                return _instance;
            }
        }
        #endregion Singleton
        
        
        #region Constructors
        private SaveManager()
        {
            m_subscribers = new List<Saveable>();  
            m_formatPolicy = new DefaultFormatter();
        }
        #endregion Constructors
        

        #region Savegame Events
        public void Save()
        {
            List<Tuple<Saveable, JObject>> savedComponents = new List<Tuple<Saveable, JObject>>();
            foreach (var subscriber in m_subscribers)
            {
                savedComponents.Add(subscriber.SaveComponents());
            }

            JObject saveFileString = m_formatPolicy.Format(savedComponents);
            saveFileString.ToString().ToJsonFile("D:\\Unity Projects\\Project Small Sandbox\\Assets\\Resources\\Saves\\savegame.json");
        }

        public void Load()
        {
            string jsonText = Resources.Load<TextAsset>("Saves/savegame").text;
            JObject saveFileObject = JObject.Parse(jsonText);
            var undo = m_formatPolicy.UndoFormatting(saveFileObject);
            
            // Check if Saveables present in the scene are on save game file
            foreach (var subscriber in m_subscribers)
            {
                // if they aren't, delete them
                if (!undo.ContainsKey(subscriber.m_componentId))
                    GameObject.Destroy(subscriber.gameObject);
                else // Else load them
                    subscriber.LoadComponents(undo[subscriber.m_componentId]);
            }
            
            //Instantiate Saveables that aren't present in the scene
        }
        #endregion Savegame Events
        
        
        #region Methods
        public void AddSubscriber(Saveable saveable)
        {
            m_subscribers.Add(saveable);
        }

        public bool RemoveSubscriber(Saveable saveable)
        {
            return m_subscribers.Remove(saveable);
        }
        #endregion Methods
    }
}