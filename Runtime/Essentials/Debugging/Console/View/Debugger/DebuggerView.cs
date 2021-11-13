using System.Collections.Generic;
using Essentials.Debugging.Console.View.Console;
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
        
        #region Singleton
        private static DebuggerView s_instance;
        #endregion Singleton
        
        #region Inspector Fields

        [Header("Dependencies")] 
        [SerializeField] private LoggerView m_loggerView;
        [SerializeField] private ConsoleView m_consoleView;
        
        [Header("Tabs Buttons")] 
        [SerializeField] private List<GameObject> m_viewTabsGameObjects;
        
        [Header("Cheat Console")] 
        [SerializeField] private TextMeshProUGUI m_consoleLogTextField;
        [SerializeField] private ContentSizeFitter m_consoleLogTextFitter;
        
        [Header("Game Logger")] 
        [SerializeField] private TextMeshProUGUI m_gameLoggerTextField;
        [SerializeField] private ContentSizeFitter m_gameLoggerTextFitter;
        
        [Header("Logger View")]
        [SerializeField] private ContentSizeFitter m_logListFitter;
        [SerializeField] private ScrollRect m_scrollRect;
        
        [Header("Debugger Content")]
        [SerializeField] private RectTransform m_content;
        #endregion Inspector Fields
        
        #region Fields

        private List<ViewTab> m_viewTabs;
        private DebugSettings m_debugSettings;
        private bool m_debuggerIsOpen;
        #endregion Fields
        
        #region Properties
        public LoggerView LoggerView => m_loggerView;
        #endregion Properties
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(transform.parent.gameObject);
                return;
            }

            s_instance = this;
            m_viewTabs = new List<ViewTab>();
            
            foreach (var tab in m_viewTabsGameObjects)
            {
                var viewTab = tab.GetComponent<ViewTab>();
                if (viewTab != null)
                {
                    viewTab.InitializeTab();
                    m_viewTabs.Add(tab.GetComponent<ViewTab>());
                }
            }
            
            UpdateViewFromSettings();
            
            if(m_viewTabs.Count > 0)
                FocusTab(m_viewTabs[0]);
            else
                Debug.LogWarning("Debugger View Tabs couldn't be initialized for some reason");
            
            m_consoleView.InitializeConsoleView();
            
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
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateViewFromSettings();            
        }
        #endif
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
            if (!m_consoleLogTextFitter || !m_gameLoggerTextFitter || !m_logListFitter)
                return;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_consoleLogTextFitter.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_gameLoggerTextFitter.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logListFitter.GetComponent<RectTransform>());
            
            m_consoleLogTextFitter.enabled = false;
            m_logListFitter.enabled = false;
            m_gameLoggerTextFitter.enabled = false;
            
            m_consoleLogTextFitter.enabled = true;
            m_logListFitter.enabled = true;
            m_gameLoggerTextFitter.enabled = true;

            m_scrollRect.normalizedPosition = Vector2.zero;
        }

        private void UpdateViewFromSettings()
        {
            m_debugSettings = DebugSettings.GetDebugSettings();
            m_content.sizeDelta = m_debugSettings.ConsoleSize;
            m_content.anchoredPosition = m_debugSettings.ConsolePosition;
            m_consoleLogTextField.fontSize = m_debugSettings.FontSize;
            m_gameLoggerTextField.fontSize = m_debugSettings.FontSize;
        }
        #endregion Utility Methods
    }
}