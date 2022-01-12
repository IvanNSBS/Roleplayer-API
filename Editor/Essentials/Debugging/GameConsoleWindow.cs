using System.Diagnostics;
using System.IO;
using INUlib.Essentials.Common;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using INUlib.Gameplay.Debugging.Settings;

namespace INUlib.Gameplay.Debugging
{
    public class GameConsoleWindow : ExtendedEditorWindow
    {
        #region Fields
        private Vector2 m_scrollPosition;
        private DebugSettings m_debugSettings;
        #endregion Fields
        
        
        #region Methods
        [MenuItem("INU lib/Debugging/Open Log Folder")]
        public static void OpenSaveFolder()
        {
            var settings = DebugSettings.GetDebugSettings();
            ProcessStartInfo startInformation = new ProcessStartInfo();
            
            if(!Directory.Exists(settings.FolderPath))
                Directory.CreateDirectory(settings.FolderPath);
            
            startInformation.FileName = settings.FolderPath;
            Process.Start(startInformation);
        }
        
        [MenuItem("INU lib/Debugging/Settings")]
        public static void OpenSettingsWindow()
        {
            GetWindow<GameConsoleWindow>("Debug Settings");
        }
        
        [MenuItem("INU lib/Debugging/Clear Persistence File")]
        public static void ClearSaveFile()
        {
            if(File.Exists(DebugSettingsPersistence.persistenceFilePath))
                File.Delete(DebugSettingsPersistence.persistenceFilePath);
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

