using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using INUlib.Gameplay.SpreadSheets;

namespace INUlib.Essentials.Spreadsheets
{
    [CustomEditor(typeof(Spreadsheet))]
    public class SpreadSheetEditor : Editor
    {
        #region Fields
        private Vector2 m_scrollPosition;
        private Spreadsheet m_spreadsheet;
        #endregion Fields
        
        #region Methods
        private void Awake() => m_spreadsheet = (Spreadsheet) target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_driveLink"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Download File", GUILayout.Height(40))) m_spreadsheet.DownloadFromDrive();

            GUIContent content = new GUIContent("Export CSV", "Exports the CSV string to the scriptable object location");
            if (GUILayout.Button(content, GUILayout.Height(40))) ExportCSV();
            EditorGUILayout.EndHorizontal();
            
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
                
            EditorGUI.BeginDisabledGroup(true);
            var data = serializedObject.FindProperty("m_data");
            EditorGUILayout.LabelField("CSV", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(data.stringValue);
            EditorGUILayout.EndScrollView();
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(String.IsNullOrEmpty(m_spreadsheet.DriveLink));
            
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            Spreadsheet spreadsheet = EditorUtility.InstanceIDToObject(instanceId) as Spreadsheet;
            if (spreadsheet != null)
            {   
                SpreadsheetWindow.Open(spreadsheet);
                return true;
            }

            return false;
        }
        #endregion Methods
        
                
        #region Helper Methods
        private void ExportCSV()
        {
            string fileLocation = AssetDatabase.GetAssetPath(m_spreadsheet);
            string location = fileLocation.Replace($"{m_spreadsheet.name}.asset", "");
            string path = $"{location}{m_spreadsheet.name}.csv";
            
            File.WriteAllText(path, m_spreadsheet.ToString());
        }
        #endregion Helper Methods
    }
}