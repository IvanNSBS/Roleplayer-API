using System.IO;
using UnityEngine;
using RPGCore.Utils;

namespace RPGCore.RPGConsole.Data
{
    public class ConsoleSettings : ScriptableObject
    {
        #region Constants
        private static string s_settingsFilePath = Path.Combine(Path.Combine("Assets", "Resources"), "Game Console Settings.asset");
        private static string s_settingsFolderPath = Path.Combine("Assets", "Resources");
        #endregion Constants
        
        #region Inspector Fields
        [Header("Console Tabs")]
        [SerializeField] private Color m_selectedColor = new Color(1f, 0.65f, 0f, 1f);
        [SerializeField] private Color m_unselectedColor = Color.white;
        
        [Header("Console Messages View")]
        [SerializeField] private Color m_userEntryColor = Color.white;
        [SerializeField] private Color m_consoleMessageColor = Color.cyan;
        [SerializeField] private Color m_errorMessageColor = Color.red;
        [SerializeField] private Color m_warningMessageColor = new Color(1f, 0.65f, 0f, 1f);

        [Header("Console Area")] 
        [SerializeField] private Vector2 m_consolePosition = new Vector2(2.5f, 2.5f);
        [SerializeField] private Vector2 m_consoleSize = new Vector2(400, 200);
        [SerializeField] private int m_logBufferSize = 256;
        [SerializeField] private float m_fontSize = 11.5f;
        #endregion Inspector Fields
        
        #region Properties
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
            return SettingsUtils.GetSettings<ConsoleSettings>(s_settingsFolderPath, s_settingsFilePath);
        }
        #endregion Methods
    }
}