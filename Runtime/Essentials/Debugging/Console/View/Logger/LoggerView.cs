using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Essentials.Debugging.Console.Data;
using Essentials.Debugging.Console.View.Debugger;
using Essentials.Debugging.Loggers;
using Essentials.Debugging.Settings;

namespace Essentials.Debugging.Console.View.Logger
{
    public class LoggerView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private DebuggerView m_debuggerView;
        [SerializeField] private TextMeshProUGUI m_loggerTextField;
        #endregion Inspector Fields

        #region Fields
        private DebugSettings m_debugSettings;
        private Queue<string> m_loggerEntries;
        #endregion Fields
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            m_debugSettings = DebugSettings.GetDebugSettings();
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

            if (m_loggerEntries.Count > m_debugSettings.LogBufferSize)
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
                    return m_debugSettings.ErrorMessageColor;
                case LogLevels.Exception:
                    return m_debugSettings.ErrorMessageColor;
                case LogLevels.Warning:
                    return m_debugSettings.WarningMessageColor;
                case LogLevels.Debug:
                    return m_debugSettings.ConsoleMessageColor;
                
                default: return m_debugSettings.ConsoleMessageColor;
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