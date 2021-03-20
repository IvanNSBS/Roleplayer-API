using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using Editor.Essentials.Common;
using Essentials.Debugging.Settings;

namespace Editor.Essentials.Debugging
{
    public class GameConsoleWindow : ExtendedEditorWindow
    {
        #region Fields
        private Vector2 m_scrollPosition;
        private DebugSettings m_debugSettings;
        #endregion Fields
        
        
        #region Methods
        [MenuItem("ZynithAPI/Debugging/Settings")]
        public static void OpenSettingsWindow()
        {
            GetWindow<GameConsoleWindow>("Debug Settings");
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
            m_debugSettings = DebugSettings.GetDebugSettings();
            serializedObject = new SerializedObject(m_debugSettings);
        }

        private void OnGUI()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            var editor = UnityEditor.Editor.CreateEditor(m_debugSettings);
            editor.OnInspectorGUI();      
            EditorGUILayout.EndScrollView();
        }
        #endregion Editor Window Methods
    }
}

