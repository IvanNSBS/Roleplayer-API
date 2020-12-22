using System;
using UnityEngine;

namespace RPGCore.Persistence.Data
{
    [Serializable]
    public class DataStoreSelector
    {
        #region Fields
        [SerializeField] private string m_fullName;
        [SerializeField] private string m_bigName;
        [SerializeField] private string m_shortName;
        #endregion Fields
                
        #region Properties
        public int ChoiceIndex { get; set; }
        public string FullName => m_fullName;
        public string BigName => m_bigName;
        public string ShortName => m_shortName;
        #endregion Properties
        
        
        #region Constructor
        public DataStoreSelector(string fullName, string bigName, string shortName)
        {
            m_fullName = fullName;
            m_bigName = bigName;
            m_shortName = shortName;
        }
        #endregion Constructor

        
        #region Methods
        public void SetData(string fullName, string bigName, string shortName)
        {
            m_fullName = fullName;
            m_bigName = bigName;
            m_shortName = shortName;
        }
        #endregion Methods
    }
}