using System;
using UnityEditor;
using UnityEngine;
using Essentials.Common;
using Essentials.SpreadSheets;
using System.Text.RegularExpressions;
using UnityEditorInternal;

namespace Essentials.Spreadsheets
{
    public class SpreadsheetWindow : ExtendedEditorWindow
    {
        #region Fields
        private Vector2 m_scrollPosition;
        private Spreadsheet m_spreadsheet;
        #endregion Fields
        
        #region Editor Methods
        public static void Open(Spreadsheet sheet)
        {
            var window = GetWindow<SpreadsheetWindow>(sheet.name);
            window.serializedObject = new SerializedObject(sheet);
            window.m_spreadsheet = sheet;
        }

        private void OnGUI()
        {
            serializedObject.Update();


            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_driveLink"));
            
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
                
            EditorGUI.BeginDisabledGroup(true);
            var data = serializedObject.FindProperty("m_data");
            EditorGUILayout.LabelField("CSV", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(data.stringValue);
            EditorGUILayout.EndScrollView();
                
            EditorGUI.EndDisabledGroup();

            
            
            EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(m_spreadsheet.DriveLink));
            if (GUILayout.Button("Download File")) m_spreadsheet.DownloadFromDrive();

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion Editor Methods
        
                
        #region Methods
        
        #endregion Methods
    }
}