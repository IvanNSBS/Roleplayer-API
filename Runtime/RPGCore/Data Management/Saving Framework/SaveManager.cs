using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using RPGCore.DataManagement.Serializers;
using RPGCore.FileManagement.SavingFramework.Formatters;

namespace RPGCore.FileManagement.SavingFramework
{
    /// <summary>
    /// Save manager handles Save game Load and Save Events.
    /// </summary>
    public class SaveManager
    {
        #region Fields
        /// <summary>
        /// List of Save/Load Events listeners
        /// </summary>
        private List<Saveable> m_subscribers;

        /// <summary>
        /// Formatting policy used to concatenate all m_subscribers
        /// json representation into a single json file to save to disk
        /// </summary>
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
        /// <summary>
        /// Save Game Event.
        /// Calls the SaveComponents Method for every Saveable
        /// that have been subscribed to the save manager and
        /// unify them through the FormattingPolicy to be a
        /// single json string and Binarize it through the
        /// BinarizePolicy
        /// </summary>
        public void Save()
        {
            Dictionary<Saveable, JObject> savedComponents = new Dictionary<Saveable, JObject>();

            foreach (var subscriber in m_subscribers)
            {
                var tuple = subscriber.SaveComponents();
                savedComponents.Add(tuple.Item1, tuple.Item2);
            }

            JObject saveFileString = m_formatPolicy.Format(savedComponents);
            saveFileString.ToString().ToJsonFile("D:\\Unity Projects\\Project Small Sandbox\\Assets\\Resources\\Saves\\savegame.json");
        }

        /// <summary>
        /// Load Save Game Event.
        /// Reads and undo binarizing through the BinarizePolicy
        /// and undo the formating through the Formatting Policy
        /// to prepare the json to be loadable from the object
        /// Saveable Component.
        /// </summary>
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
        /// <summary>
        /// Add subscriber to SaveGame listeners
        /// </summary>
        /// <param name="saveable">New Subscriber</param>
        public void AddSubscriber(Saveable saveable)
        {
            m_subscribers.Add(saveable);
        }

        /// <summary>
        /// Removes Saveable from the event listener list.
        /// </summary>
        /// <param name="saveable">Saveable to remove</param>
        /// <returns>True if Saveable was removed. False otherwise</returns>
        public bool RemoveSubscriber(Saveable saveable)
        {
            return m_subscribers.Remove(saveable);
        }
        #endregion Methods
    }
}