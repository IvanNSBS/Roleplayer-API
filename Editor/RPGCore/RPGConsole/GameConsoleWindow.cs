using UnityEditor;
using UnityEngine;
using Editor.Common;
using UnityEditor.Callbacks;
using RPGCore.RPGConsole.Data;

namespace Editor.RPGCore.RPGConsole
{
    public class GameConsoleWindow : ExtendedEditorWindow
    {
        #region Fields
        private Vector2 m_scrollPosition;
        private ConsoleSettings m_consoleSettings;
        #endregion Fields
        
        
        #region Methods
        [MenuItem("RPG-API/Game Console/Settings")]
        public static void OpenSettingsWindow()
        {
            GetWindow<GameConsoleWindow>("Game Console Settings");
        }
        
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            GameConsoleWindow persistenceSettings = EditorUtility.InstanceIDToObject(instanceId) as GameConsoleWindow;
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
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
            serializedObject = new SerializedObject(m_consoleSettings);
        }

        private void OnGUI()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            var editor = UnityEditor.Editor.CreateEditor(m_consoleSettings);
            editor.OnInspectorGUI();      
            EditorGUILayout.EndScrollView();
        }
        #endregion Editor Window Methods
    }
}

