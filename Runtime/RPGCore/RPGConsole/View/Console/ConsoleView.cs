using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using RPGCore.RPGConsole.Data;
using RPGCore.RPGConsole.View.Debugger;
using RPGCore.RPGConsole.Commands.BuiltinCommands;

namespace RPGCore.RPGConsole.View.Console
{
    public class ConsoleView : MonoBehaviour, IConsoleView
    {
        #region Serializable Fields

        [Header("Debugger View")] 
        [SerializeField] private DebuggerView m_debuggerView;
            
        [Header("Keyboard Input")]
        [SerializeField] private TMP_InputField m_consoleInputField;

        [Header("Buttons")] 
        [SerializeField] private Button m_sendButton;
        [SerializeField] private Button m_clearButton;
        [SerializeField] private Button m_closeButton;

        [Header("Log Text")] 
        [SerializeField] private TextMeshProUGUI m_logTextField;
        #endregion Serializable Fields
        
        #region Fields
        private Queue<string> m_formattedMessagesQueue;
        private ConsoleSettings m_consoleSettings;
        private ZynithConsole m_zynithConsole;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Start()
        {
            InputSystem.EnableDevice(Keyboard.current);
            
            m_formattedMessagesQueue = new Queue<string>();
            m_logTextField.text = "";
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
            m_zynithConsole = new ZynithConsole(m_consoleSettings, this);
            m_consoleInputField.onSubmit.AddListener(m_zynithConsole.HandleLogInputCommand);

            CommandRegistry.RegisterContainer(new ConsoleHelperCommandsContainer(m_zynithConsole, m_debuggerView));
            SetupButtons();
        }

        private void OnDisable()
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        private void SetupButtons()
        {
            m_consoleInputField.onSelect.AddListener(str => InputSystem.DisableDevice(Keyboard.current));
            m_consoleInputField.onDeselect.AddListener(str => InputSystem.EnableDevice(Keyboard.current));
            
            m_sendButton.onClick.AddListener(() => m_zynithConsole.HandleLogInputCommand(m_consoleInputField.text));
            m_clearButton.onClick.AddListener(() =>
            {
                m_zynithConsole.ConsoleCommands["clearConsole"].Invoke(null);
                m_zynithConsole.ConsoleCommands["clearLogger"].Invoke(null);
            });
            m_closeButton.onClick.AddListener(m_debuggerView.CloseDebugger);
        }

        public void OnEntrySubmitted()
        {
            m_consoleInputField.text = "";
            FocusInputText();
        }

        public void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType)
        {
            string formattedEntry = FormatInputString(logEntry, entryType) + "\n";
            m_formattedMessagesQueue.Enqueue(formattedEntry);
            m_logTextField.text += formattedEntry;
            m_debuggerView.ScrollToBottom();
        }

        public void ConsoleQueueExceeded()
        {
            m_logTextField.text = m_logTextField.text.Substring(m_formattedMessagesQueue.Dequeue().Length);
            m_debuggerView.ScrollToBottom();
        }

        public void ConsoleCleared()
        {
            m_logTextField.text = "";
            m_formattedMessagesQueue.Clear();
            m_debuggerView.ScrollToBottom();
        }
        #endregion Methods
        
        
        #region Utility Methods
        private void FocusInputText()
        {
            m_consoleInputField.Select();
            m_consoleInputField.ActivateInputField();
        }

        private Color GetColorFromConsoleEntry(ConsoleEntryType entryType)
        {
            switch (entryType)
            {
                case ConsoleEntryType.Error:
                    return m_consoleSettings.ErrorMessageColor;
                case ConsoleEntryType.Warning:
                    return m_consoleSettings.WarningMessageColor;
                case ConsoleEntryType.ConsoleMessage:
                    return m_consoleSettings.ConsoleMessageColor;
                case ConsoleEntryType.UserInput:
                    return m_consoleSettings.UserEntryColor;
                
                default: return m_consoleSettings.ConsoleMessageColor;
            }
        }
        
        private string FormatInputString(string inputString, ConsoleEntryType entryType)
        {
            string colorHex = "#"+ColorUtility.ToHtmlStringRGB(GetColorFromConsoleEntry(entryType));
            return $"<color={colorHex}>> {inputString}</color>";
        }
        #endregion Utility Methods
    }
}