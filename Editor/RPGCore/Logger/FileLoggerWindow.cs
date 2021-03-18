using UnityEditor;
using Editor.Common;
using RPGCore.Loggers;
using UnityEditor.Callbacks;

namespace Editor.RPGCore.Logger
{
    public class FileLoggerWindow : ExtendedEditorWindow
    {
        #region Fields
        private LoggerSettings m_consoleSettings;
        #endregion Fields
        
        #region Methods
        [MenuItem("RPG-API/Logger/Settings")]
        public static void OpenSettingsWindow()
        {
            GetWindow<FileLoggerWindow>("Zynith Logger Settings");
        }
        
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            LoggerSettings persistenceSettings = EditorUtility.InstanceIDToObject(instanceId) as LoggerSettings;
            if (persistenceSettings != null)
            {   
                OpenSettingsWindow();
                return true;
            }

            return false;
        }
        #endregion Methods
        
        #region Editor Window Methods
        private void Awake()
        {
            m_consoleSettings = LoggerSettings.GetLoggerSettings();
            serializedObject = new SerializedObject(m_consoleSettings);
        }

        private void OnGUI()
        {
            var editor = UnityEditor.Editor.CreateEditor(m_consoleSettings);
            editor.OnInspectorGUI();      
        }
        #endregion Editor Window Methods
    }
}