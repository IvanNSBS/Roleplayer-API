using System;
using System.Diagnostics;
using System.IO;
using INUlib.UEditor.Common;
using UnityEditor;
using UnityEditor.Callbacks;
using INUlib.Core.Persistence.Data;
using UnityEngine;

namespace INUlib.UEditor.Core.Persistence.Windows
{
    public class PersistenceSettingsWindow : ExtendedEditorWindow
    {
        #region Fields
        private PersistenceSettings m_persistenceSettings;
        private bool m_folded;

        private Type[] m_types;
        private string[] m_smallNames;
        private string[] m_bigNames;
        private string[] m_fullNames;
        private Vector2 m_scrollPosition;
        #endregion Fields
        
        
        #region Methods
        [MenuItem("INU lib/Persistence/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            var settings = PersistenceSettings.GetPersistenceSettings();
            ProcessStartInfo startInformation = new ProcessStartInfo();
            
            if(!Directory.Exists(settings.FileFolder))
                Directory.CreateDirectory(settings.FileFolder);
            
            startInformation.FileName = settings.FileFolder;
            Process.Start(startInformation);
        }

        [MenuItem("INU lib/Persistence/Clear Save File")]
        public static void ClearSaveFile()
        {
            var settings = PersistenceSettings.GetPersistenceSettings();
            if(File.Exists(settings.FilePath))
                File.Delete(settings.FilePath);
        }
        
        [MenuItem("INU lib/Persistence/Settings")]
        public static void OpenSettingsWindow()
        {
            GetWindow<PersistenceSettingsWindow>("Persistence Settings");
        }
        
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            PersistenceSettings persistenceSettings = EditorUtility.InstanceIDToObject(instanceId) as PersistenceSettings;
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
            var settings = PersistenceSettings.GetPersistenceSettings();
            m_persistenceSettings = settings;
            serializedObject = new SerializedObject(settings);
        }

        private void OnGUI()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            var editor = Editor.CreateEditor(m_persistenceSettings);
            editor.OnInspectorGUI();      
            EditorGUILayout.EndScrollView();
            
            Apply();
        }
        #endregion Editor Window Methods

        
        #region Utility Methods
        private void Apply()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        #endregion Utility Methods
    }
}