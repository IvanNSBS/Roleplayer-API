using System;
using UnityEngine;
using UnityEditor;

namespace Essentials.Common
{
    public class ExtendedEditorWindow : EditorWindow
    {
        #region Fields
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;

        private string m_selectedPropertyPath;
        protected SerializedProperty selectedProperty;
        #endregion Fields
        
        #region Methods
        protected void DrawProperties(SerializedProperty property, bool drawChildren)
        {
            string lastPropertyPath = String.Empty;
            foreach (SerializedProperty prop in property)
            {
                if (prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
                {
                    // EditorGUILayout.BeginHorizontal();
                    // prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
                    // EditorGUILayout.EndHorizontal();
                    //
                    // if (prop.isExpanded)
                    // {
                    //     EditorGUI.indentLevel++;
                    //     DrawProperties(prop, drawChildren);
                    //     EditorGUI.indentLevel--;
                    // }

                    EditorGUILayout.PropertyField(prop, true);
                    
                    lastPropertyPath = prop.propertyPath;
                }
                else
                {
                    if(!String.IsNullOrEmpty(lastPropertyPath) && prop.propertyPath.Contains(lastPropertyPath))
                        continue;
                    lastPropertyPath = prop.propertyPath;
                    EditorGUILayout.PropertyField(prop, drawChildren);
                }
            }
        }

        protected void DrawSideBar(SerializedProperty property)
        {
            foreach (SerializedProperty prop in property)
            {
                if (GUILayout.Button(prop.displayName))
                {
                    m_selectedPropertyPath = prop.propertyPath;
                }
            }

            if (!String.IsNullOrEmpty(m_selectedPropertyPath))
            {
                selectedProperty = serializedObject.FindProperty(m_selectedPropertyPath);
            }
        }
        #endregion Methosd
    }
}