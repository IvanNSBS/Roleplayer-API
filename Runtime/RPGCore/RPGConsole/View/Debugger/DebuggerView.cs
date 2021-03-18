using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using RPGCore.RPGConsole.Data;
using RPGCore.RPGConsole.View.Logger;

namespace RPGCore.RPGConsole.View.Debugger
{
    public class DebuggerView : MonoBehaviour
    {
        #region Constants
        private const Key OPEN_CONSOLE_KEY = Key.F3;
        #endregion Constants
        
        #region Inspector Fields
        [Header("Tabs Buttons")] 
        [SerializeField] private List<ViewTab> m_viewTabs;
        
        [Header("Console Logger")] 
        [SerializeField] private TextMeshProUGUI m_consoleLogTextField;
        [SerializeField] private ContentSizeFitter m_consoleLogTextFitter;
        
        [Header("Zynith Logger")] 
        [SerializeField] private TextMeshProUGUI m_zynithLogTextField;
        [SerializeField] private ContentSizeFitter m_zynithLogTextFitter;
        
        [Header("Logger View")]
        [SerializeField] private ContentSizeFitter m_logListFitter;
        [SerializeField] private ScrollRect m_scrollRect;
        
        [Header("Debugger Content")]
        [SerializeField] private RectTransform m_content;
        #endregion Inspector Fields
        
        #region Fields
        private LoggerView m_loggerView;
        private ConsoleSettings m_consoleSettings;
        private bool m_debuggerIsOpen;
        #endregion Fields
        
        #region Properties
        public LoggerView LoggerView => m_loggerView;
        #endregion Properties
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            m_loggerView = GetComponentInChildren<LoggerView>();
            UpdateViewFromSettings();
            FocusTab(m_viewTabs[0]);
        }

        private void Update()
        {
            if ( !m_debuggerIsOpen && Keyboard.current[OPEN_CONSOLE_KEY].wasPressedThisFrame )
                OpenDebugger();
        }
        
        private void OnValidate()
        {
            UpdateViewFromSettings();            
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        public void FocusTab(ViewTab tab)
        {
            if (!m_viewTabs.Contains(tab)) return;
            
            foreach (var viewTab in m_viewTabs)
            {
                if(viewTab == tab)
                    viewTab.SelectTab();
                else
                    viewTab.DeselectTab();
            }            
        }
        
        public void OpenDebugger()
        {
            m_debuggerIsOpen = true;
            m_content.gameObject.SetActive(true);
        }

        public void CloseDebugger()
        {
            m_debuggerIsOpen = false;
            m_content.gameObject.SetActive(false);
        }
        #endregion Methods
        
        
        #region Utility Methods
        public void ScrollToBottom()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_consoleLogTextFitter.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_zynithLogTextField.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logListFitter.GetComponent<RectTransform>());
            
            m_consoleLogTextFitter.enabled = false;
            m_logListFitter.enabled = false;
            m_zynithLogTextFitter.enabled = false;
            
            m_consoleLogTextFitter.enabled = true;
            m_logListFitter.enabled = true;
            m_zynithLogTextFitter.enabled = true;

            m_scrollRect.normalizedPosition = Vector2.zero;
        }

        private void UpdateViewFromSettings()
        {
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
            m_content.sizeDelta = m_consoleSettings.ConsoleSize;
            m_content.anchoredPosition = m_consoleSettings.ConsolePosition;
            m_consoleLogTextField.fontSize = m_consoleSettings.FontSize;
        }
        #endregion Utility Methods
    }
}