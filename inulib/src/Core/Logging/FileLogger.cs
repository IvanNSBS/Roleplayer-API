﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace INUlib.Core
{
    /// <summary>
    /// LogPolicy that writes logs to a file
    /// </summary>
    public class FileLogger
    {
        #region Fields
        private readonly string _fileExtension;
        private readonly string _fileSubfolder;
        private readonly string _fileName;
        private readonly uint _maxLogFiles;
        private readonly FileStream m_fileStream;
        private readonly List<string> m_logEntries;
        #endregion Fields
        
        #region Properties
        private string AppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public string LogFolderPath => Path.Join(AppDataPath, _fileSubfolder);
        #endregion

        #region Log Methods
        /// <summary>
        /// Constructor for FilePolicy
        /// </summary>
        public FileLogger(string subFolderName, string fileName, string fileExtension, uint maxLogFiles)
        {
            _fileExtension = fileExtension;
            _fileSubfolder = subFolderName;
            _fileName = fileName;
            _maxLogFiles = maxLogFiles;
            
            m_logEntries = new List<string>();
            int lastFileIndex = GetLastFileIndex(LogFolderPath);

            string completeFileName = $"{fileName}_{lastFileIndex}{fileExtension}";
            
            var completePath = Path.Combine(LogFolderPath, completeFileName);

            if (maxLogFiles <= 0)
                return;

            if (!Directory.Exists(LogFolderPath))
            {
                Directory.CreateDirectory(LogFolderPath);
            }
            else
            {
                List<string> files = Directory.GetFiles(LogFolderPath).ToList();
                if (files.Count > 0 && files.Count >= maxLogFiles)
                {
                    files.Sort();
                    int deleteCount = System.Math.Max(files.Count - (int)maxLogFiles + 1, 0);
                    for(int i = 0; i < deleteCount; i++)
                        File.Delete(files[i]);
                }
            }

            Logger.onLogReceived += Log;
            m_fileStream = File.Create(completePath);
        }
        
        ~FileLogger()
        {
            m_fileStream.Flush();
            m_fileStream.Close();
            Logger.onLogReceived -= Log;
        }

        private void Log(string logEntry, LogLevel level, string formattedLogEntry)
        {
            string formattedMsg = $"{formattedLogEntry}\n";
            AddLogEntry(formattedMsg);
        }

        public void Flush() => m_fileStream.Flush();
        #endregion Destructor
        
        
        #region Utility Methods
        /// <summary>
        /// Utility method to add text to a file from a string
        /// </summary>
        /// <param name="value"></param>
        private void AddLogEntry(string value)
        {
            if (_maxLogFiles <= 0)
                return;

            byte[] info = new UTF8Encoding(true).GetBytes(value);
            m_fileStream.Write(info, 0, info.Length);
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
    }
}