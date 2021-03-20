using System.Collections.Generic;
using Essentials.Debugging.Console.View.Logger;
using Essentials.Debugging.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Essentials.Debugging.Console.View.Debugger
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
        private DebugSettings m_debugSettings;
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
            
            CloseDebugger();
            
            DontDestroyOnLoad(transform.parent.gameObject);
        }

        private void Update()
        {
            if (Keyboard.current[OPEN_CONSOLE_KEY].wasPressedThisFrame)
            {
                if(!m_debuggerIsOpen)
                    OpenDebugger();
                else
                    CloseDebugger();
            }
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
            m_debugSettings = DebugSettings.GetDebugSettings();
            m_content.sizeDelta = m_debugSettings.ConsoleSize;
            m_content.anchoredPosition = m_debugSettings.ConsolePosition;
            m_consoleLogTextField.fontSize = m_debugSettings.FontSize;
            m_zynithLogTextField.fontSize = m_debugSettings.FontSize;
        }
        #endregion Utility Methods
    }
}