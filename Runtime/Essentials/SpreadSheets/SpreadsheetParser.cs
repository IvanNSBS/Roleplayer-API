using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.Utils.Extensions;
using UnityEngine;

namespace Essentials.SpreadSheets
{
    public static class SpreadsheetParser
    {
        #region Fields
        public static string exampleCSV =  "id,name,weight,damage,canCrit\n"+
                                            "wep_01,Iron Sword,10.3,20,TRUE\n"+
                                            "wep_02,Silver Sword,5.4,25,TRUE\n"+
                                            "wep_03,Gold Sword,9.3,29,TRUE\n"+
                                            "wep_04,Diamond Sword,13.7,34,TRUE\n"+
                                            "wep_05,Invalid Entry,,,";
        #endregion Fields
        
        #region Methods
        public static List<T> ReadSheet<T>(string csvFile) where T : class
        {
            var lines = csvFile.Split('\n');
            if (lines.Length < 2)
                return null;
            
            var columns = lines[0].Split(',');
            var rows = lines.SubArray(1);

            List<T> values = new List<T>();
            
            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                var rowValues = row.Split(',');

                Dictionary<string, string> rowColumn = new Dictionary<string, string>();
                
                for(int j = 0; j < rowValues.Length; j++)
                {
                    var col = rowValues[j];
                    
                    // if(string.IsNullOrEmpty(col))
                    //     Debug.LogWarning($"Row {i}: <{columns[j]}> entry is empty");
                    // else
                    //     Debug.Log(col);
                    
                    rowColumn.Add(columns[j], col);
                }
                
                values.Add(EntriesToClass<T>(rowColumn));
            }
            
            return values;
        }
        #endregion Methods
        
        
        #region Helper Methods
        private static T EntriesToClass<T>(Dictionary<string, string> entries) where T : class
        {
            BindingFlags validFields = BindingFlags.Public | BindingFlags.NonPublic;

            var fields = typeof(T).GetFields().
                Where(x => x.GetCustomAttribute<CSVColumnAttribute>() != null);

            T result = Activator.CreateInstance<T>();
            
            foreach (var field in fields)
            {
                string columnName = field.GetCustomAttribute<CSVColumnAttribute>().name;
                string finalName = string.IsNullOrEmpty(columnName) ? field.Name : columnName; 
                Debug.Log($"Final Name: {finalName}");

                if (entries.ContainsKey(finalName) && !string.IsNullOrEmpty(entries[finalName]))
                {
                    field.SetValue(result, Convert.ChangeType(entries[finalName], field.FieldType));
                }
            }

            return result;
        }
        #endregion Helper Methods
    }
}