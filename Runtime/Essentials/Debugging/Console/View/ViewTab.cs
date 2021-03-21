using Essentials.Debugging.Console.Data;
using Essentials.Debugging.Console.View.Debugger;
using Essentials.Debugging.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Debugging.Console.View
{
    [RequireComponent(typeof(Button))]
    public class ViewTab : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private DebuggerView m_debuggerView; 
        [SerializeField] private TextMeshProUGUI m_targetTextField;
        [SerializeField] private TextMeshProUGUI m_buttonText;
        #endregion Inspector Fields
        
        #region Fields
        private DebugSettings m_settings;
        #endregion Fields
        
        
        #region Inspector Fields
        public void InitializeTab()
        {
            m_settings = DebugSettings.GetDebugSettings();
            GetComponent<Button>().onClick.AddListener(() => m_debuggerView.FocusTab(this));
        }
        #endregion Inspector Fields
        

        #region Methods
        public void SelectTab()
        {
            m_targetTextField.gameObject.SetActive(true);
            m_buttonText.color = m_settings.SelectedColor;
        }

        public void DeselectTab()
        {
            m_targetTextField.gameObject.SetActive(false);
            m_buttonText.color = m_settings.UnselectedColor;
        }
        #endregion Methods
    }
}