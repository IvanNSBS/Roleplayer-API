using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RPGCore.Loggers
{
    /// <summary>
    /// LogPolicy that writes logs to a file
    /// </summary>
    class FilePolicy : LogPolicy
    {
        #region Fields
        private readonly FileStream m_fileStream;
        private readonly List<string> m_logEntries;
        #endregion Fields
        
        #region Constructors
        /// <summary>
        /// Constructor for FilePolicy
        /// </summary>
        /// <param name="settings">Configuration file for log a policy</param>
        public FilePolicy(LoggerSettings settings) : base(settings)
        {
            m_logEntries = new List<string>();
            int lastFileIndex = GetLastFileIndex(settings.FolderPath);

            string fileName = $"{settings.LogFileName}_{lastFileIndex}{settings.FileExtension}";
            
            var completePath = Path.Combine(settings.FolderPath, fileName);

            if (!Directory.Exists(settings.FolderPath))
                Directory.CreateDirectory(settings.FolderPath);
            
            m_fileStream = File.Create(completePath);
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
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            m_fileStream.Write(info, 0, info.Length);
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

        private void LogToUnityConsole(LogLevels level, string logEntry)
        {
            switch (level)
            {
                case LogLevels.Debug:   
                    Debug.Log(logEntry); 
                    break;
                case LogLevels.Warning: 
                    Debug.LogWarning(logEntry); 
                    break;
                case LogLevels.Error:   
                    Debug.LogError(logEntry);
                    break;
                case LogLevels.Exception:
                    Debug.LogError(logEntry);
                    break;
                default:                
                    Debug.Log(logEntry); 
                    break;
            }
        }
        #endregion Utility Methods
        
        
        #region LogPolicy Methods
        public override string Log(LogLevels level, string logEntry, bool fromUnityCallback = false)
        {
            string formattedMsg = $"{DateTime.Now}  {level}\t:...{logEntry}\n";

            AddLogEntry(formattedMsg);
            
            if (loggerSettings.LogToUnityConsole && !fromUnityCallback)
                LogToUnityConsole(level, logEntry);

            return formattedMsg;
        }
        #endregion LogPolicy Methods
    }
}