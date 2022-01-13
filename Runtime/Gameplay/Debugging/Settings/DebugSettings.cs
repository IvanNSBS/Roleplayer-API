using System;
using System.IO;
using INUlib.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace INUlib.Gameplay.Debugging.Settings
{
    public static class DebugSettingsPersistence
    {
        public static string persistenceFileFolder = Path.Combine(Application.persistentDataPath, "Debug Console");
        public static string persistenceFilePath = Path.Combine(persistenceFileFolder, "console settings.json");
    }
    
    public class DebugSettings : ScriptableObject
    {
        #region Constants
        private static string s_settingsFileName = "INUlib Debugging Settings";
        private static string s_settingsSubFolderName = "INUlib";
        #endregion Constants
        
        #region Inspector Fields
        [Header("Log File Settings")] 
        [SerializeField] [Range(0, 20)] private int m_maxNumberOfLogFiles = 10;
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
        [SerializeField] private float m_fontSize = 13.5f;

        [Header("Console Persistence")] 
        [SerializeField] private bool m_storeConsoleSettingsOnDisk = true;
        #endregion Inspector Fields

        #region Properties
        public Color SelectedColor => m_selectedColor;
        public Color UnselectedColor => m_unselectedColor;
        public Vector2 ConsoleSize => m_consoleSize;
        public Vector2 ConsolePosition => m_consolePosition;
        public int LogBufferSize => m_logBufferSize;
        public float FontSize
        {
            get => m_fontSize;
            set => m_fontSize = value;
        }
        public Color UserEntryColor => m_userEntryColor;
        public Color ConsoleMessageColor => m_consoleMessageColor;
        public Color ErrorMessageColor => m_errorMessageColor;
        public Color WarningMessageColor => m_warningMessageColor;
        public string FileExtension => m_logFileExtension;
        public string LogFileName => m_logFileName;
        public string FolderPath =>  String.IsNullOrEmpty(m_logSubFolder) ? 
            Application.persistentDataPath : Path.Combine(Application.persistentDataPath, m_logSubFolder);
        public int MaxNumberOfLogFiles => m_maxNumberOfLogFiles;
        #endregion Properties


        #region Scriptable Object Methods
        private void OnEnable()
        {
            if (File.Exists(DebugSettingsPersistence.persistenceFilePath) && m_storeConsoleSettingsOnDisk)
            {
                string fileAsString = File.ReadAllText(DebugSettingsPersistence.persistenceFilePath);
                JObject serializedSettings = JObject.Parse(fileAsString);
                
                float fontSize = (float)serializedSettings["fontSize"];
                float xPos     = (float)serializedSettings["position"]["x"];
                float yPos     = (float)serializedSettings["position"]["y"];
                float xSize    = (float)serializedSettings["size"]["x"];
                float ySize    = (float)serializedSettings["size"]["y"];

                m_fontSize = fontSize;
                m_consolePosition = new Vector2(xPos, yPos);
                m_consoleSize = new Vector2(xSize, ySize);
            }
            
            Application.quitting += StoreSettings;
        }

        private void OnDisable()
        {
            Application.quitting -= StoreSettings;
        }
        #endregion Scriptable Object Methods
        
        
        #region Methods
        public void OverwriteConsoleSize(Vector2 newSize)
        {
            m_consoleSize = newSize;
        }
        
        public void OverwriteConsolePosition(Vector2 newPosition)
        {
            m_consolePosition = newPosition;
        }
        
        private void StoreSettings()
        {
            if (!m_storeConsoleSettingsOnDisk)
                return;
            
            JObject jObject = JObject.FromObject(new
            {
                fontSize = FontSize,
                position = new
                {
                    m_consolePosition.x,
                    m_consolePosition.y,
                },
                size = new
                {
                    m_consoleSize.x,
                    m_consoleSize.y,
                }
            });

            if (!Directory.Exists(DebugSettingsPersistence.persistenceFileFolder)) 
                Directory.CreateDirectory(DebugSettingsPersistence.persistenceFileFolder);
            
            File.WriteAllText(DebugSettingsPersistence.persistenceFilePath, jObject.ToString());
        }

        public static DebugSettings GetDebugSettings()
        {
            return SettingsUtils.GetSettings<DebugSettings>(s_settingsSubFolderName, s_settingsFileName);
        }
        #endregion Methods
    }
}