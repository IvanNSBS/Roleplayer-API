using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using INUlib.Core;
using INUlib.Utils.Extensions;

namespace INUlib.BackendToolkit.SpreadSheets
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
        /// <summary>
        /// Parses a CSV File String.
        /// Note: Will Only parse fields marked with CSVColumn Attribute.
        /// Will Return Null if a field is marked as Important and can't be parsed
        /// </summary>
        /// <param name="csvFile">CSV File String</param>
        /// <typeparam name="T">Type that will be created</typeparam>
        /// <returns>List of the Parsed T type</returns>
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

                var entry = EntriesToClass<T>(rowColumn);
                if (entry == null)
                    return null;
                
                values.Add(entry);
            }
            
            return values;
        }
        #endregion Methods
        
        
        #region Helper Methods
        private static T EntriesToClass<T>(Dictionary<string, string> entries) where T : class
        {
            // BindingFlags validFields = BindingFlags.Public | BindingFlags.NonPublic;
            var fields = typeof(T).GetFields().
                Where(x => x.GetCustomAttribute<CSVColumnAttribute>() != null);

            T result = Activator.CreateInstance<T>();
            
            foreach (var field in fields)
            {
                CSVColumnAttribute attribute = field.GetCustomAttribute<CSVColumnAttribute>();
                string columnName = attribute.name;
                string finalName = string.IsNullOrEmpty(columnName) ? field.Name : columnName;
                bool isImportantField = attribute.important;

                if (entries.ContainsKey(finalName) && !string.IsNullOrEmpty(entries[finalName])) {
                    try {
                        field.SetValue(result, Convert.ChangeType(entries[finalName], field.FieldType));
                    }
                    catch (Exception e) {
                        Logger.LogError(e.ToString());
                        return null;
                    }
                }
                else if(isImportantField) {
                    Logger.LogError(
                        $"CSV Field <{finalName}> is marked as important"+ 
                        "and wasn't found in CSV or couldn't be parsed"
                    );
                    return null;
                }
            }

            return result;
        }
        #endregion Helper Methods
    }
}