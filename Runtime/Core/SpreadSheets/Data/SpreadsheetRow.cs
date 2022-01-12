using System;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Core.SpreadSheets
{
    [Serializable]
    public class SpreadsheetRow
    {
        #region Fields
        [SerializeField] private string[] m_columns;
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