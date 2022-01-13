using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using INUlib.BackendToolkit.Persistence.Data;
using UnityEditor.SceneManagement;
using INUlib.BackendToolkit.Persistence.GameObjects;

namespace INUlib.UEditor.Core.Persistence
{
    [CustomEditor(typeof(PersistentSceneGameObject))]
    public class PersistentSceneGameObjectEditor : UnityEditor.Editor
    {
        #region Fields
        private PersistentGameObject m_persistentGameObject;
        private PersistenceSettings m_persistenceSettings;
        #endregion Fields
        
        
        #region Editor Methods 
        public override void OnInspectorGUI()
        {
            if (m_persistentGameObject == null)
                return;
            
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            var csIcon = EditorGUIUtility.FindTexture("cs Script Icon");
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((PersistentGameObject)target), typeof(PersistentGameObject), false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Unique Id:", m_persistentGameObject.ObjectId, textFieldStyle);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_prefabManagerElement"));
            
            var surrogates = m_persistentGameObject.GetComponentsInChildren<GameObjectSurrogate>();
            
            GUILayout.BeginVertical(textFieldStyle);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Source Object", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label("Surrogate", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(3f);
            
            // Draw Surrogates
            foreach (var surrogate in surrogates)
            {
                // If Id has not been set or there's id collision, generate a new Id for surrogate.
                if (String.IsNullOrEmpty(surrogate.Id) || surrogates.Any(x => x.Id == surrogate.Id && x != surrogate))
                {
                    surrogate.Id = Guid.NewGuid().ToString();
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(surrogate);
                        EditorSceneManager.MarkSceneDirty(m_persistentGameObject.gameObject.scene);
                    }
                }
                
                EditorGUILayout.BeginHorizontal();
                {
                    string label = $"{surrogate.gameObject.name}";
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(label, MonoScript.FromMonoBehaviour(surrogate), typeof(GameObjectSurrogate), false);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
                
            }
            GUILayout.EndVertical();
            
            
            GUILayout.Space(10);
            
            
            if(GUILayout.Button("Generate Unique ID", GUILayout.Height(30)))
            {
                m_persistentGameObject.ObjectId = m_persistentGameObject.gameObject.name + "_" + Guid.NewGuid();
            }

            if (GUILayout.Button("Create Prefab Element", GUILayout.Height(30)))
            {
                if (HasPrefabElement())
                {
                    if (EditorUtility.DisplayDialog("Overwrite Prefab Element Id?",
                        "Are you sure you want to overwrite the Prefab Element Id? " +
                        "Current save file will loose references to this GameObject and won't load them.", 
                        "Replace", "Cancel"))
                    {
                        UpdatePrefabElementID();
                    }
                }
                else
                {
                    CreateNewPrefabElement();
                }
            }
            
            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(m_persistentGameObject);
                foreach (var surrogate in surrogates)
                    EditorUtility.SetDirty(surrogate);

                EditorSceneManager.MarkSceneDirty(m_persistentGameObject.gameObject.scene);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void Awake()
        {
            m_persistenceSettings = PersistenceSettings.GetPersistenceSettings();
            m_persistentGameObject = (PersistentGameObject) target;
            if(String.IsNullOrEmpty(m_persistentGameObject.ObjectId))
                m_persistentGameObject.ObjectId = m_persistentGameObject.gameObject.name + "_" + Guid.NewGuid();
        }
        #endregion Editor Methods
        
        
        #region Utility Methods
        private bool HasPrefabElement()
        {
            return m_persistentGameObject.m_prefabManagerElement != null;
        }
        
        private bool IsPrefab()
        {
            var prefabType = PrefabUtility.GetPrefabAssetType(m_persistentGameObject.gameObject);
            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(m_persistentGameObject.gameObject);
            return prefabType == PrefabAssetType.Regular && prefabStatus == PrefabInstanceStatus.Connected;
        }

        private string GetPrefabElementPath()
        {
            string elementPath = m_persistenceSettings.FullPrefabElementFolder;
            string prefabElementName = $"{m_persistentGameObject.gameObject.name}.asset";
            return Path.Combine(elementPath, prefabElementName);
        }
        
        private GameObject GetPrefab()
        {
            if (IsPrefab())
            {
                var pr = PrefabUtility.GetCorrespondingObjectFromSource(m_persistentGameObject.gameObject);
                return pr;
            }
            
            string prefabFolder = m_persistenceSettings.PrefabFolder;
            string prefabName = $"{m_persistentGameObject.gameObject.name}.prefab";
            string assetPath = Path.Combine(prefabFolder, prefabName);
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            
            if (!Directory.Exists(prefabFolder))
                Directory.CreateDirectory(prefabFolder);
            
            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(m_persistentGameObject.gameObject, uniqueAssetPath, InteractionMode.UserAction);
            return prefab;
        }
        
        private void CreateNewPrefabElement()
        {
            // If there's no directory for prefabElement Folder, create it
            if (!Directory.Exists(m_persistenceSettings.FullPrefabElementFolder))
                Directory.CreateDirectory(m_persistenceSettings.FullPrefabElementFolder);
            
            PrefabManagerElement asset = CreateInstance<PrefabManagerElement>();
            
            string prefabId = Guid.NewGuid().ToString();

            var prefab = GetPrefab();
            asset.SetPrefab(prefabId, prefab);
            
            string assetPath = GetPrefabElementPath();
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            
            AssetDatabase.CreateAsset(asset, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            
            m_persistentGameObject.m_prefabManagerElement = asset;
            prefab.GetComponent<PersistentGameObject>().m_prefabManagerElement = asset;
            PrefabUtility.SavePrefabAsset(prefab);
        }

        private void UpdatePrefabElementID()
        {
            m_persistentGameObject.m_prefabManagerElement.UpdateId(Guid.NewGuid().ToString());
        }
        #endregion Utility Methods
    }
}