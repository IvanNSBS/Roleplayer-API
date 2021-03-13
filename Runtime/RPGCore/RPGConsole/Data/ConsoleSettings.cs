using System.IO;
using UnityEditor;
using UnityEngine;

namespace RPGCore.RPGConsole.Data
{
    public class ConsoleSettings : ScriptableObject
    {
        #region Constants
        private static string settingsFilePath = Path.Combine(Path.Combine("Assets", "Resources"), "Game Console Settings.asset");
        private static string settingsFolderPath = Path.Combine("Assets", "Resources");
        #endregion Constants
        
        #region Inspector Fields

        [Header("Console Area")] 
        [SerializeField] private Vector2 m_consolePosition = new Vector2(2.5f, 2.5f);
        [SerializeField] private Vector2 m_consoleSize = new Vector2(400, 200);
        [SerializeField] private int m_logBufferSize = 256;
        [SerializeField] private float m_fontSize = 11.5f;

        [Header("Console Messages View")]
        [SerializeField] private Color m_userEntryColor = Color.white;
        [SerializeField] private Color m_consoleMessageColor = Color.cyan;
        [SerializeField] private Color m_errorMessageColor = Color.red;
        [SerializeField] private Color m_warningMessageColor = new Color(1f, 0.65f, 0f, 1f);
        #endregion Inspector Fields
        
        #region Properties
        public Vector2 ConsoleSize => m_consoleSize;
        public Vector2 ConsolePosition => m_consolePosition;
        public int LogBufferSize => m_logBufferSize;
        public float FontSize => m_fontSize;
        public Color UserEntryColor => m_userEntryColor;
        public Color ConsoleMessageColor => m_consoleMessageColor;
        public Color ErrorMessageColor => m_errorMessageColor;
        public Color WarningMessageColor => m_warningMessageColor;
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
        
        public static ConsoleSettings GetConsoleSettings()
        {
            if (!Directory.Exists(settingsFolderPath))
                Directory.CreateDirectory(settingsFolderPath);
            
            var settings = AssetDatabase.LoadAssetAtPath<ConsoleSettings>(settingsFilePath);
            if (settings != null)
                return settings;
            
            ConsoleSettings asset = CreateInstance<ConsoleSettings>();

            AssetDatabase.CreateAsset(asset, settingsFilePath);
            AssetDatabase.SaveAssets();

            return asset;
        }
        #endregion Methods
    }
}