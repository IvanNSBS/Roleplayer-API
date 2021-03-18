using System;
using System.Diagnostics;
using System.IO;
using RPGCore.Utils;
using UnityEditor;
using UnityEngine;

namespace RPGCore.Loggers
{
    public class LoggerSettings : ScriptableObject
    {
        #region Constants
        private static string s_settingsFilePath = Path.Combine(Path.Combine("Assets", "Resources"), "Logger Settings.asset");
        private static string s_settingsFolderPath = Path.Combine("Assets", "Resources");
        #endregion Constants
        
        #region Fields
        [Header("File Settings")]
        [SerializeField] private string m_logSubFolder = "Logs";
        [SerializeField] private string m_logFileName = "log";
        [SerializeField] private string m_logFileExtension = ".log";
        #endregion Fields
        
        #region Properties
        public string FileExtension => m_logFileExtension;
        public string LogFileName => m_logFileName;
        public string FolderPath =>  String.IsNullOrEmpty(m_logSubFolder) ? 
            Application.persistentDataPath : Path.Combine(Application.persistentDataPath, m_logSubFolder);
        #endregion Properties
        
        #region Methods
        public static LoggerSettings GetLoggerSettings()
        {
            return SettingsUtils.GetSettings<LoggerSettings>(s_settingsFolderPath, s_settingsFilePath);
        }
        
        [MenuItem("RPG-API/Logger/Open Log Folder")]
        public static void OpenSaveFolder()
        {
            var settings = GetLoggerSettings();
            ProcessStartInfo startInformation = new ProcessStartInfo();
            
            if(!Directory.Exists(settings.FolderPath))
                Directory.CreateDirectory(settings.FolderPath);
            
            startInformation.FileName = settings.FolderPath;
            Process.Start(startInformation);
        }
        #endregion Methods
    }
}