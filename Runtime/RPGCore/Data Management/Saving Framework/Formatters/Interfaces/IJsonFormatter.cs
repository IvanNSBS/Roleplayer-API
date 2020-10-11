using System;
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
        /// Organizes all subscribed Saveables json representation(JObject)
        /// into a single json that represents the save file
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
        Dictionary<string, JObject> UndoFormatting(JObject saveGameJson);
    }
}