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
        JObject Format(List<Tuple<Saveable, JObject>> componentsToSave);
        Dictionary<string, JObject> UndoFormatting(JObject saveGameJson);
    }
}