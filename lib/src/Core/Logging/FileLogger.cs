using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using INUlib.Core;

namespace INUlib.Gameplay.Debugging.Loggers
{
    /// <summary>
    /// LogPolicy that writes logs to a file
    /// </summary>
    class FileLogger
    {
        #region Fields
        private readonly string _fileExtension;
        private readonly string _fileFolder;
        private readonly string _fileName;
        private readonly uint _maxLogFiles;
        private readonly FileStream m_fileStream;
        private readonly List<string> m_logEntries;
        #endregion Fields
        
        #region Constructors
        /// <summary>
        /// Constructor for FilePolicy
        /// </summary>
        /// <param name="settings">Configuration file for log a policy</param>
        public FileLogger(string fileFolder, string fileName, string fileExtension, uint maxLogFiles)
        {
            _fileExtension = fileExtension;
            _fileFolder = fileFolder;
            _fileName = fileName;
            _maxLogFiles = maxLogFiles;
            
            m_logEntries = new List<string>();
            int lastFileIndex = GetLastFileIndex(fileFolder);

            string completeFileName = $"{fileName}_{lastFileIndex}{fileExtension}";
            
            var completePath = Path.Combine(fileFolder, completeFileName);

            if (maxLogFiles > 0)
            {
                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }
                else
                {
                    List<string> files = Directory.GetFiles(fileFolder).ToList();
                    if (files.Count > 0 && files.Count >= maxLogFiles)
                    {
                        files.Sort();
                        int deleteCount = Math.Max(files.Count - (int)maxLogFiles + 1, 0);
                        for(int i = 0; i < deleteCount; i++)
                            File.Delete(files[i]);
                    }
                }
                
                m_fileStream = File.Create(completePath);
            }
        }
        #endregion Constructors

        #region Destructor
        ~FileLogger()
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
            if (_maxLogFiles > 0)
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
        public string Log(LogLevel level, string logEntry)
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