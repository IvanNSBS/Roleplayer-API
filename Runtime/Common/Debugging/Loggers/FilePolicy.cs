using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using INUlib.Common.Debugging.Loggers.Interfaces;
using INUlib.Common.Debugging.Settings;
using UnityEngine;

namespace INUlib.Common.Debugging.Loggers
{
    /// <summary>
    /// LogPolicy that writes logs to a file
    /// </summary>
    class FilePolicy : LogPolicy
    {
        #region Fields

        private DebugSettings m_settings;
        private readonly FileStream m_fileStream;
        private readonly List<string> m_logEntries;
        #endregion Fields
        
        #region Constructors
        /// <summary>
        /// Constructor for FilePolicy
        /// </summary>
        /// <param name="settings">Configuration file for log a policy</param>
        public FilePolicy(DebugSettings settings) : base(settings)
        {
            m_settings = settings;
            m_logEntries = new List<string>();
            int lastFileIndex = GetLastFileIndex(settings.FolderPath);

            string fileName = $"{settings.LogFileName}_{lastFileIndex}{settings.FileExtension}";
            
            var completePath = Path.Combine(settings.FolderPath, fileName);

            if (settings.MaxNumberOfLogFiles > 0)
            {
                if (!Directory.Exists(settings.FolderPath))
                {
                    Directory.CreateDirectory(settings.FolderPath);
                }
                else
                {
                    List<string> files = Directory.GetFiles(settings.FolderPath).ToList();
                    if (files.Count > 0 && files.Count >= settings.MaxNumberOfLogFiles)
                    {
                        files.Sort();
                        int deleteCount = Mathf.Max(files.Count - settings.MaxNumberOfLogFiles + 1, 0);
                        for(int i = 0; i < deleteCount; i++)
                            File.Delete(files[i]);
                    }
                }
                
                m_fileStream = File.Create(completePath);
            }
        }
        #endregion Constructors

        #region Destructor
        ~FilePolicy()
        {
            m_fileStream.Close();
        }
        #endregion Destructor
        
        
        #region Utility Methods
        /// <summary>
        /// Utility method to add text to a file from a string
        /// </summary>
        /// <param name="value"></param>
        private void AddLogEntry(string value)
        {
            if (m_settings.MaxNumberOfLogFiles > 0)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                m_fileStream.Write(info, 0, info.Length);
            }
            m_logEntries.Add(value);
        }

        private int GetLastFileIndex(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return 0;
            
            string[] files = Directory.GetFiles(folderPath);
            int lastIndex = 0;
            string fileName = "";

            foreach (string file in files)
            {
                fileName = Path.GetFileName(file);
                
                fileName = Regex.Match(fileName, @"\d+").Value;

                int index = int.Parse(fileName);
                if (index > lastIndex) lastIndex = index;
            }

            return lastIndex + 1;
        }
        #endregion Utility Methods
        
        
        #region LogPolicy Methods
        public override string Log(LogLevels level, string logEntry)
        {
            var now = DateTime.Now;
            string hour = now.Hour.ToString("00");
            string minute = now.Minute.ToString("00");
            string second = now.Second.ToString("00");
            string formattedMsg = $"[{hour}:{minute}:{second}] {level}: {logEntry}\n";

            AddLogEntry(formattedMsg);
            
            return formattedMsg;
        }
        #endregion LogPolicy Methods
    }
}