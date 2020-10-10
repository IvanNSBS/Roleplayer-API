using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework.Formatters
{
    public class DefaultFormatter : IJsonFormatter
    {
        public JObject Format(List<Tuple<Saveable, JObject>> componentsToSave)
        {
            JObject result = new JObject();
            foreach (var saveComponent in componentsToSave)
            {
                Saveable saveable = saveComponent.Item1;
                JObject jsonRepresentation = saveComponent.Item2;
                
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