﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    /// <summary>
    /// DefaultFormatter simply puts all objects into a json.
    /// No scene or any other data is considered in the operations
    /// </summary>
    public class DefaultFormatter : IJsonFormatter
    {
        public JObject Format(Dictionary<Saveable, JObject> componentsToSave)
        {
            JObject result = new JObject();
            foreach (var saveComponent in componentsToSave)
            {
                Saveable saveable = saveComponent.Key;
                JObject jsonRepresentation = saveComponent.Value;
                
                result.Add(saveable.m_componentId, jsonRepresentation);
            }

            return result;
        }

        public Dictionary<string, JObject> UndoFormatting(JObject saveGameJson)
        {
            Dictionary<string, JObject> result = new Dictionary<string, JObject>();
            foreach (var token in saveGameJson)
            {
                result.Add(token.Key, token.Value as JObject);
            }

            return result;
        }
    }
}