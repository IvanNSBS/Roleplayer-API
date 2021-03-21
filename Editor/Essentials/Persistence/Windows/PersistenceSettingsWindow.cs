using System;
using System.Linq;
using Essentials.Common;
using UnityEditor;
using UnityEditor.Callbacks;
using Essentials.Persistence.Data;
using Essentials.Persistence.Interfaces;
using UnityEngine;

namespace Essentials.Persistence.Windows
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
        #endregion Fields
        
        
        #region Methods
        [MenuItem("ZynithAPI/Persistence/Settings")]
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
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal(GUILayout.Height(26));
            m_folded = EditorGUILayout.Foldout(m_folded, "Registered Data Stores", true);    
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(25)))
            {
                string fullName = m_fullNames[0];
                string bigName = m_bigNames[0];
                string shortName = m_smallNames[0];
                
                DataStoreSelector selector = new DataStoreSelector(fullName, bigName, shortName);
                selector.ChoiceIndex = 0;
                
                m_persistenceSettings.RegisteredDataStores.Add(selector);
                
            }
            if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                int count = m_persistenceSettings.RegisteredDataStores.Count; 
                m_persistenceSettings.RegisteredDataStores.RemoveAt(count - 1);
            }
            EditorGUILayout.EndHorizontal();

            if (m_folded)
            {
                EditorGUI.indentLevel++;
                foreach (var registeredDataStore in m_persistenceSettings.RegisteredDataStores)
                {
                    registeredDataStore.ChoiceIndex = EditorGUILayout.Popup(
                        registeredDataStore.ShortName, registeredDataStore.ChoiceIndex, m_bigNames);

                    string fullName = m_fullNames[registeredDataStore.ChoiceIndex];
                    string bigName = m_bigNames[registeredDataStore.ChoiceIndex];
                    string shortName = m_smallNames[registeredDataStore.ChoiceIndex];
                    registeredDataStore.SetData(fullName, bigName, shortName);
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
            if (m_types != null)
                return;
            
            var type = typeof(DataStore);
            m_types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract).ToArray();

            m_fullNames = new string[m_types.Length];
            m_bigNames = new string[m_types.Length];
            m_smallNames = new string[m_types.Length];
            
            for (int i = 0; i < m_types.Length; i++)
            {
                m_fullNames[i] = m_types[i].FullName + ", " + m_types[i].Assembly.GetName().Name;
                m_bigNames[i] = m_types[i].FullName;
                m_smallNames[i] = m_types[i].Name;
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