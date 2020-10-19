using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using RPGCore.FileManagement.SavingFramework;
using UnityEditor.SceneManagement;

namespace Editors.RPGCore.SavingFramework
{
    [CustomEditor(typeof(Saveable))]
    public class SaveableEditor : Editor
    {
        #region Fields
        private Saveable m_saveable;
        #endregion Fields
        
        
        #region Editor Methods 
        public override void OnInspectorGUI()
        {
            if (m_saveable == null)
                return;
            
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            var csIcon = EditorGUIUtility.FindTexture("cs Script Icon");
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Saveable)target), typeof(Saveable), false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Unique Id:", m_saveable.ComponentId, textFieldStyle);
            EditorGUI.EndDisabledGroup();
            m_saveable.m_saveableOptions = (SaveableOptions)EditorGUILayout.EnumPopup("Saveable Options", m_saveable.m_saveableOptions);
            m_saveable.m_prefabReferenceId = EditorGUILayout.TextField("Prefab ID", m_saveable.m_prefabReferenceId);
            
            var surrogates = m_saveable.GetComponentsInChildren<ISaveableData>();
            
            GUILayout.BeginVertical(textFieldStyle);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Surrogates ({surrogates.Count()})", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            foreach (var surrogate in surrogates)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal(textFieldStyle);
                GUILayout.Label(csIcon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.Label(surrogate.GetType().Name + " (Script)");
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            
            if(GUILayout.Button("Generate Unique ID", GUILayout.Height(30)))
            {
                m_saveable.ComponentId = m_saveable.gameObject.name + "_" + Guid.NewGuid();
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_saveable);
                EditorSceneManager.MarkSceneDirty(m_saveable.gameObject.scene);
            }
        }

        private void Awake()
        {
            m_saveable = (Saveable) target;
            if(String.IsNullOrEmpty(m_saveable.ComponentId))
                m_saveable.ComponentId = m_saveable.gameObject.name + "_" + Guid.NewGuid();
        }
        #endregion Editor Methods
    }
}