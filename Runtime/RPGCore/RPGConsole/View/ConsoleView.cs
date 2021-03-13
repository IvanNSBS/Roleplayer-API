using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using RPGCore.RPGConsole.Data;

namespace RPGCore.RPGConsole.View
{
    public class ConsoleView : MonoBehaviour, IConsoleView
    {
        #region Constants
        private const Key OPEN_CONSOLE_KEY = Key.F3;
        #endregion Constants
        
        #region Serializable Fields
        [Header("Keyboard Input")]
        [SerializeField] private TMP_InputField m_consoleInputField;

        [Header("Buttons")] 
        [SerializeField] private Button m_sendButton;
        [SerializeField] private Button m_clearButton;
        [SerializeField] private Button m_closeButton;

        [Header("Log Text")] 
        [SerializeField] private ContentSizeFitter m_logFitter;
        [SerializeField] private ContentSizeFitter m_logTextFitter;
        [SerializeField] private TextMeshProUGUI m_logTextField;
        [SerializeField] private ScrollRect m_scrollRect;

        [Header("Cheat Console Content")]
        [SerializeField] private RectTransform m_content;
        #endregion Serializable Fields
        
        #region Fields
        private bool m_consoleIsOpen;
        private ConsoleSettings m_consoleSettings;
        private ZynithConsole m_zynithConsole;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            UpdateViewFromSettings();

            m_logTextField.text = "";
            InputSystem.EnableDevice(Keyboard.current);
            m_zynithConsole = new ZynithConsole(m_consoleSettings, this);
            m_consoleInputField.onSubmit.AddListener(m_zynithConsole.HandleLogInputCommand);
            
            SetupButtons();
            OpenConsole();
        }

        private void OnDisable()
        {
            if(m_consoleIsOpen)
                InputSystem.EnableDevice(Keyboard.current);
        }
        
        private void Update()
        {
            if ( !m_consoleIsOpen && Keyboard.current[OPEN_CONSOLE_KEY].wasPressedThisFrame )
                OpenConsole();
        }

        private void OnValidate()
        {
            UpdateViewFromSettings();            
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        private void SetupButtons()
        {
            m_sendButton.onClick.AddListener(() => m_zynithConsole.HandleLogInputCommand(m_consoleInputField.text));
            m_clearButton.onClick.AddListener(() =>
            {
                m_zynithConsole.ConsoleCommands["clear"].Invoke(null);
            });
            m_closeButton.onClick.AddListener(CloseConsole);
        }
        
        
        public void OpenConsole()
        {
            m_consoleIsOpen = true;
            m_content.gameObject.SetActive(true);
            InputSystem.DisableDevice(Keyboard.current);
        }

        public void CloseConsole()
        {
            m_consoleIsOpen = false;
            m_content.gameObject.SetActive(false);
            InputSystem.EnableDevice(Keyboard.current);
        }

        public void OnEntrySubmitted()
        {
            m_consoleInputField.text = "";
            FocusInputText();
        }

        public void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType)
        {
            string formattedEntry = FormatInputString(logEntry, entryType) + "\n";
            m_logTextField.text += formattedEntry;
            ScrollToBottom();
        }

        public void ConsoleEntryRemoved(string logEntry)
        {
            m_logTextField.text = m_logTextField.text.Substring(logEntry.Length);
        }

        public void ConsoleCleared()
        {
            m_logTextField.text = "";
            ScrollToBottom();
        }
        #endregion Methods
        
        
        #region Utility Methods
        private void FocusInputText()
        {
            m_consoleInputField.Select();
            m_consoleInputField.ActivateInputField();
        }
        
        private void ScrollToBottom()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logTextFitter.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logFitter.GetComponent<RectTransform>());
            m_logTextFitter.enabled = false;
            m_logFitter.enabled = false;
            m_logTextFitter.enabled = true;
            m_logFitter.enabled = true;
            m_scrollRect.normalizedPosition = Vector2.zero;
        }

        private void UpdateViewFromSettings()
        {
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
            m_content.sizeDelta = m_consoleSettings.ConsoleSize;
            m_content.anchoredPosition = m_consoleSettings.ConsolePosition;
            m_logTextField.fontSize = m_consoleSettings.FontSize;
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