using UnityEditor;
using UnityEngine;
using INUlib.UEditor.Common;
using INUlib.BackendToolkit.SpreadSheets;

namespace INUlib.UEditor.Core.Spreadsheets
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
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            var editor = Editor.CreateEditor(m_spreadsheet);
            editor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        #endregion Editor Methods
    }
}