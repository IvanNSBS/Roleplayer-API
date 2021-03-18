using TMPro;
using UnityEngine;
using RPGCore.Loggers;
using RPGCore.RPGConsole.Data;
using System.Collections.Generic;
using RPGCore.RPGConsole.View.Debugger;

namespace RPGCore.RPGConsole.View.Logger
{
    public class LoggerView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private DebuggerView m_debuggerView;
        [SerializeField] private TextMeshProUGUI m_loggerTextField;
        #endregion Inspector Fields

        #region Fields
        private ConsoleSettings m_consoleSettings;
        private Queue<string> m_loggerEntries;
        #endregion Fields
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
            m_loggerEntries = new Queue<string>();
        }

        private void OnEnable()
        {
            ZynithLogger.logMessageReceived.AddListener(AddEntryToLogger);
        }

        private void OnDisable()
        {
            ZynithLogger.logMessageReceived.RemoveListener(AddEntryToLogger);
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        public void AddEntryToLogger(string logMessage, LogLevels logLevel)
        {
            string formattedMessage = FormatInputString(logLevel, logMessage);
            
            m_loggerEntries.Enqueue(formattedMessage);
            m_loggerTextField.text += formattedMessage;

            if (m_loggerEntries.Count > m_consoleSettings.LogBufferSize)
                m_loggerTextField.text = m_loggerTextField.text.Substring(m_loggerEntries.Dequeue().Length);
            
            m_debuggerView.ScrollToBottom();
        }

        public void ClearLog()
        {
            m_loggerTextField.text = "";
            m_loggerEntries.Clear();
            m_debuggerView.ScrollToBottom();
        }
        #endregion Methods
        
        
        #region Utility Methods
        private Color GetColorFromConsoleEntry(LogLevels logLevel)
        {
            switch (logLevel)
            {
                case LogLevels.Error:
                    return m_consoleSettings.ErrorMessageColor;
                case LogLevels.Exception:
                    return m_consoleSettings.ErrorMessageColor;
                case LogLevels.Warning:
                    return m_consoleSettings.WarningMessageColor;
                case LogLevels.Debug:
                    return m_consoleSettings.ConsoleMessageColor;
                
                default: return m_consoleSettings.ConsoleMessageColor;
            }
        }
        
        private string FormatInputString(LogLevels entryType, string inputString)
        {
            string colorHex = "#"+ColorUtility.ToHtmlStringRGB(GetColorFromConsoleEntry(entryType));
            return $"<color={colorHex}>> {inputString}</color>";
        }
        #endregion Utility Methods
    }
}