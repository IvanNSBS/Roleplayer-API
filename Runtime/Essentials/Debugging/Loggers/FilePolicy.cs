using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Essentials.Debugging.Loggers.Interfaces;
using Essentials.Debugging.Settings;

namespace Essentials.Debugging.Loggers
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

            if (settings.CreateLogFile)
            {
                if (!Directory.Exists(settings.FolderPath))
                    Directory.CreateDirectory(settings.FolderPath);
                
                m_fileStream = File.Create(completePath);
            }
        }
        #endregion Constructors

        #region Destructor
        ~FilePolicy()
        {
            // Debug.Log("File Policy Destructor called....");
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
            if (m_settings.CreateLogFile)
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

        private void LogToUnityConsole(LogLevels level, string logEntry)
        {
            switch (level)
            {
                case LogLevels.Debug:   
                    UnityEngine.Debug.Log(logEntry); 
                    break;
                case LogLevels.Warning: 
                    UnityEngine.Debug.LogWarning(logEntry); 
                    break;
                case LogLevels.Error:   
                    UnityEngine.Debug.LogError(logEntry);
                    break;
                case LogLevels.Exception:
                    UnityEngine.Debug.LogError(logEntry);
                    break;
                default:                
                    UnityEngine.Debug.Log(logEntry); 
                    break;
            }
        }
        #endregion Utility Methods
        
        
        #region LogPolicy Methods
        public override string Log(LogLevels level, string logEntry, bool fromUnityCallback = false)
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