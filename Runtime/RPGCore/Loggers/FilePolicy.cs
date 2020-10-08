using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace RPGCore.Loggers
{
    /// <summary>
    /// LogPolicy that writes logs to a file
    /// </summary>
    class FilePolicy : ILogPolicy
    {
        #region Fields
        private Dictionary<string, Action<LogLevels, string>> m_options;
        private FileStream m_fileStream;
        #endregion Fields
        
        #region Constructors
        /// <summary>
        /// Constructor for FileLog Policy
        /// </summary>
        /// <param name="logFileFolder">Folder to create the file</param>
        /// <param name="logFileName">File Name. Will remove extension and use .log if there's any</param>
        public FilePolicy(string logFileFolder, string logFileName)
        {
            int lastIndex = logFileName.IndexOf(".");
            string fileName = logFileName;
            if (lastIndex > 0)
            {
                string logFileWithoutExtension = fileName.Substring(0, lastIndex);
                fileName = logFileWithoutExtension;
            }
            fileName = fileName + ".log";
            var completePath = Path.Combine(logFileFolder, fileName);
            
            if(File.Exists(completePath))
                File.Delete(completePath);

            m_fileStream = File.Create(completePath);
                
            m_options = new Dictionary<string, Action<LogLevels, string>>();
            m_options.Add("--debug", (logLevel, message) =>
            {
                switch (logLevel)
                {
                    case LogLevels.Debug:
                        Debug.Log(message);
                        break;
                    case LogLevels.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogLevels.Error:
                        Debug.LogError(message);
                        break;
                    default:
                        Debug.Log(message);
                        break;
                }
            });
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
        private void AddText(string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            m_fileStream.Write(info, 0, info.Length);
        }
        #endregion Utility Methods
        
        
        #region LogPolicy Methods
        /// <summary>
        /// Effectively logs to a file
        /// </summary>
        /// <param name="level">the log level. Warning, Debug, etc</param>
        /// <param name="message">The message to write</param>
        /// <param name="args">additional arguments for the log command</param>
        public void Log(LogLevels level, string message, params string[] args)
        {
            string formattedMsg = $"{DateTime.Now}  {level}\t\t:...{message}\n";
            
            AddText(formattedMsg);
            
            foreach (var arg in args)
            {
                if(m_options.ContainsKey(arg))
                    m_options[arg].Invoke(level, formattedMsg);
            }
        }
        #endregion LogPolicy Methods
    }
}