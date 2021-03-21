using System;
using System.IO;
using Lib.Utils;
using UnityEngine;

namespace Essentials.Debugging.Settings
{
    public class DebugSettings : ScriptableObject
    {
        #region Constants
        private static string s_settingsFileName = "Zynith Debugging Settings";
        private static string s_settingsSubFolderName = "Debugging";
        #endregion Constants
        
        #region Inspector Fields

        [Header("Log File Settings")] 
        [SerializeField] private bool m_createLogFile = false;
        [SerializeField] private string m_logSubFolder = "Logs";
        [SerializeField] private string m_logFileName = "log";
        [SerializeField] private string m_logFileExtension = ".log";
        
        [Header("Console Tabs")]
        [SerializeField] private Color m_selectedColor = new Color(1f, 0.65f, 0f, 1f);
        [SerializeField] private Color m_unselectedColor = Color.white;
        
        [Header("Console Messages View")]
        [SerializeField] private Color m_userEntryColor = Color.cyan;
        [SerializeField] private Color m_consoleMessageColor = Color.white;
        [SerializeField] private Color m_errorMessageColor = Color.red;
        [SerializeField] private Color m_warningMessageColor = new Color(1f, 0.65f, 0f, 1f);

        [Header("Console Area")] 
        [SerializeField] private Vector2 m_consolePosition = new Vector2(2.5f, 2.5f);
        [SerializeField] private Vector2 m_consoleSize = new Vector2(400, 200);
        [SerializeField] private int m_logBufferSize = 256;
        [SerializeField] private float m_fontSize = 11.5f;
        #endregion Inspector Fields

        #region Properties
        public bool CreateLogFile => m_createLogFile;
        public Color SelectedColor => m_selectedColor;
        public Color UnselectedColor => m_unselectedColor;
        public Vector2 ConsoleSize => m_consoleSize;
        public Vector2 ConsolePosition => m_consolePosition;
        public int LogBufferSize => m_logBufferSize;
        public float FontSize => m_fontSize;
        public Color UserEntryColor => m_userEntryColor;
        public Color ConsoleMessageColor => m_consoleMessageColor;
        public Color ErrorMessageColor => m_errorMessageColor;
        public Color WarningMessageColor => m_warningMessageColor;
        public string FileExtension => m_logFileExtension;
        public string LogFileName => m_logFileName;
        public string FolderPath =>  String.IsNullOrEmpty(m_logSubFolder) ? 
            Application.persistentDataPath : Path.Combine(Application.persistentDataPath, m_logSubFolder);
        #endregion Properties
        
        #region Methods
        public void OverwriteConsoleSize(Vector2 newSize)
        {
            m_consoleSize = newSize;
        }
        
        public void OverwriteConsolePosition(Vector2 newPosition)
        {
            m_consolePosition = newPosition;
        }

        public static DebugSettings GetDebugSettings()
        {
            return SettingsUtils.GetSettings<DebugSettings>(s_settingsSubFolderName, s_settingsFileName);
        }
        #endregion Methods
    }
}