using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Essentials.SpreadSheets
{
    [CreateAssetMenu(fileName = "Spreadsheet", menuName = "Zynith/Spreadsheet", order = 0)]
    public class Spreadsheet : ScriptableObject
    {
        #region Fields

        [Header("Google Drive")] 
        [SerializeField] private string m_driveLink;

        [Header("Object Data")]
        [SerializeField] private string m_data;
        // [SerializeField] public SpreadsheetRow m_columns;
        // [SerializeField] private List<string> m_rows;
        #endregion Fields
        
        #region Properties
        public string DriveLink => m_driveLink;
        #endregion Properties
        
        
        #region Methods
        public override string ToString()
        {
            // string columns = string.Join(",", m_columns)+'\n';

            // string result = columns;
            return m_data;
        }

        public void DownloadFromDrive()
        {
            UnityWebRequest www = UnityWebRequest.Get(GetExportLink(this));
            UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

            asyncOp.completed += OnFinishDownload;
        }

        private void OnFinishDownload(AsyncOperation op)
        {
            var web = op as UnityWebRequestAsyncOperation;
            m_data = web.webRequest.downloadHandler.text;
            Debug.Log(m_data);
        }
        #endregion Methods
        
        
        #region Helper Methods
        private static string GetExportLink(Spreadsheet sheet)
        {
            Regex regex = new Regex(@"/d/(.*?)/");

            string id = regex.Match(sheet.DriveLink).Groups[1].Value;
            var csvLink = $"https://docs.google.com/spreadsheets/d/{id}/gviz/tq?tqx=out:csv&sheet={sheet.name}";
            return csvLink;
        }
        #endregion Helper Methods
    }
}