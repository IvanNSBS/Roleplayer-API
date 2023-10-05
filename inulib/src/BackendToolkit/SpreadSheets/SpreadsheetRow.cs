using System;
using System.Collections.Generic;

namespace INUlib.BackendToolkit.SpreadSheets
{
    [Serializable]
    public class SpreadsheetRow
    {
        #region Fields
        private string[] m_columns;
        #endregion Fields
        
        #region Properties
        public string[] Columns
        {
            get => m_columns;
            set => m_columns = value;
        }
        #endregion Properties
    }
}