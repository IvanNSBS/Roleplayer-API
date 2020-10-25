﻿using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    /// <summary>
    /// Interface that defines how a formatter
    /// will organize a list Saveables and their jsonResult tuple
    /// into a complete save file .json and how to undo this
    /// operation so Saveable Components can load their
    /// objects again 
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// Property that contains the json cache of the current save game
        /// </summary>
        JObject SaveGameCache { get; set; }
        
        /// <summary>
        /// Organizes all SaveManager subscribed Saveables json representations(JObject)
        /// into a single json that represents the complete save file
        /// </summary>
        /// <param name="componentsToSave">
        /// Dictionary that has the Saveable Component as key and
        /// it's json representation(JObject) as value
        /// </param>
        /// <returns>A single json(JObject) that represents the save file</returns>
        JObject Format(Dictionary<Saveable, JObject> componentsToSave);
        
        /// <summary>
        /// Undo the formatting.
        /// </summary>
        /// <param name="saveGameJson">
        ///  Json(JObject) that represents the save game file
        /// </param>
        /// <returns>
        /// Dictionary that has the Saveable Component ID as key and
        /// it's json representation(JObject) as value
        /// </returns>
       List<SavedObject> GetSceneSavedGameObjects(int sceneBuildIndex);
    }

    public class SavedObject
    {
        #region Fields
        public string id;
        public JObject jsonRepresentation;
        #endregion Fields
        
        
        #region Constructors
        public SavedObject(string id, JObject jsonRepresentation)
        {
            this.id = id;
            this.jsonRepresentation = jsonRepresentation;
        }
        #endregion Constructors
    }
}