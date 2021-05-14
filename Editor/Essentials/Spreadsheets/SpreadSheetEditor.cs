using UnityEditor;
using UnityEngine;
using Essentials.SpreadSheets;
using UnityEditor.Callbacks;

namespace Essentials.Spreadsheets
{
    [CustomEditor(typeof(Spreadsheet))]
    public class SpreadSheetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Editor", GUILayout.Height(30)))
            {
                SpreadsheetWindow.Open((Spreadsheet)target);
            }
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
    }
}