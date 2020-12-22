using System;
using System.Linq;
using UnityEditor;
using Editor.Common;
using UnityEditor.Callbacks;
using RPGCore.Persistence.Data;
using RPGCore.Persistence.Interfaces;
using UnityEngine;

namespace Editor.RPGCore.Persistence.Windows
{
    public class PersistenceSettingsWindow : ExtendedEditorWindow
    {
        #region Fields
        private PersistenceSettings m_persistenceSettings;
        private bool m_folded;
        private string[] m_fullNames;
        private string[] m_shortNames;
        private int m_choiceIndex;
        #endregion Fields
        
        
        #region Methods
        [MenuItem("RPG-API/Persistence/Settings")]
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
            Start();
            var settings = PersistenceSettings.GetPersistenceSettings();
            m_persistenceSettings = settings;
            serializedObject = new SerializedObject(settings);
        }

        private void OnGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_saveSubFolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_fileName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_fileExtension"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_encryptionMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_prefabFolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_prefabElementFolder"));

            // ------------ Dropdown List ----------------
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Data Store", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal(GUILayout.Height(26));
            m_folded = EditorGUILayout.Foldout(m_folded, "Registered Data Stores", true);    
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                DataStoreSelector selector = new DataStoreSelector(m_fullNames[0], m_shortNames[0]);
                m_persistenceSettings.RegisteredDataStores.Add(selector);
                
            }
            if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                int count = m_persistenceSettings.RegisteredDataStores.Count; 
                m_persistenceSettings.RegisteredDataStores.RemoveAt(count - 1);
            }
            EditorGUILayout.EndHorizontal();

            if (!m_folded)
            {
                EditorGUI.indentLevel++;
                foreach (var registeredDataStore in m_persistenceSettings.RegisteredDataStores)
                {
                    registeredDataStore.ChoiceIndex = EditorGUILayout.Popup(
                        registeredDataStore.ShortName, registeredDataStore.ChoiceIndex, m_fullNames);
                    
                    registeredDataStore.SetData(m_fullNames[registeredDataStore.ChoiceIndex], m_shortNames[registeredDataStore.ChoiceIndex]);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            
            Apply();
        }
        #endregion Editor Window Methods

        
        #region Utility Methods
        private void Start()
        {
            if (m_fullNames != null)
                return;
            
            var type = typeof(DataStore);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract).ToArray();

            int size = types.Count();
            m_fullNames = new string[size];
            m_shortNames = new string[size];
            
            for(int i = 0; i < size; i++)
            {
                var currentType = types[i];
                m_fullNames[i] = currentType.FullName + ", " + currentType.Assembly.GetName().Name;
                m_shortNames[i] = currentType.Name;
            }
        }
        
        private void Apply()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        #endregion Utility Methods
    }
}