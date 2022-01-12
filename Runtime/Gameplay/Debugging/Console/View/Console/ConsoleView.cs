using System.Collections.Generic;
using INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands;
using INUlib.Gameplay.Debugging.Console.Data;
using INUlib.Gameplay.Debugging.Console.View.Debugger;
using INUlib.Gameplay.Debugging.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace INUlib.Gameplay.Debugging.Console.View.Console
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
        private DebugSettings m_debugSettings;
        private CheatConsole m_cheatConsole;
        #endregion Fields


        #region MonoBehaviour Methods
        public void InitializeConsoleView()
        {
            InputSystem.EnableDevice(Keyboard.current);
            
            m_formattedMessagesQueue = new Queue<string>();
            m_logTextField.text = "";
            m_debugSettings = DebugSettings.GetDebugSettings();
            m_cheatConsole = new CheatConsole(m_debugSettings, this);
            m_consoleInputField.onSubmit.AddListener(m_cheatConsole.HandleLogInputCommand);

            CommandRegistry.RegisterContainer(new ConsoleHelperCommandsContainer(m_cheatConsole, m_debuggerView));
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
            
            m_sendButton.onClick.AddListener(() => m_cheatConsole.HandleLogInputCommand(m_consoleInputField.text));
            m_clearButton.onClick.AddListener(() =>
            {
                m_cheatConsole.HandleLogInputCommand("clearLogger");
                m_cheatConsole.HandleLogInputCommand("clearConsole");
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
                    return m_debugSettings.ErrorMessageColor;
                case ConsoleEntryType.Warning:
                    return m_debugSettings.WarningMessageColor;
                case ConsoleEntryType.ConsoleMessage:
                    return m_debugSettings.ConsoleMessageColor;
                case ConsoleEntryType.UserInput:
                    return m_debugSettings.UserEntryColor;
                
                default: return m_debugSettings.ConsoleMessageColor;
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