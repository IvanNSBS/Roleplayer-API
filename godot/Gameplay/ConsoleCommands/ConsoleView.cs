using INUlib.Core;
using INUlib.Gameplay.Debugging.Console;
using INUlib.Gameplay.Debugging.Console.Data;

namespace Godot.INUlib.Gameplay
{
    [Tool]
    public partial class ConsoleView : Control
    {
        #region Inspector Fields
        [Export] private Button _sendBtn; 
        [Export] private Button _clearBtn; 
        [Export] private Button _closeBtn; 
        [Export] private LineEdit _input;
        [Export] private Control _consoleContainer;
        [Export] private TabBar _tabs;
        [Export] private RichTextLabel _loggerLogsText;
        [Export] private RichTextLabel _consoleLogsText;
        #endregion

        #region Fields
        private CheatConsole _console;
        private int consoleTab = 1;
        private int logsTab = 2;
        #endregion


        #region Godot Methods
        public override void _Ready()
        {
            _input.Text = "";
            _loggerLogsText.Text = "";
            _consoleLogsText.Text = "";

            if(_sendBtn != null) _sendBtn.Pressed += OnClickSend;
            if(_closeBtn != null) _closeBtn.Pressed += OnClickClose;
            if(_clearBtn != null) _clearBtn.Pressed += OnClickClear;
            if(_input != null) _input.TextSubmitted += OnInputTextSubmited;
            if(_tabs != null) _tabs.TabChanged += OnTabsChanged;

            Logger.onLogReceived += OnLogEntryAdded;
            _console = new CheatConsole(256);
            _console.onEntryAddedToLog += OnConsoleEntryAdded;
            _console.onConsoleCleared += ClearLogText;

            _console.Init();
        }

        public override void _Process(double delta)
        {
            _sendBtn.Disabled = _input == null || _input.Text == "";
            if(Input.IsKeyPressed(Key.F10))
                _consoleContainer.Visible = !_consoleContainer.Visible;
        }
        #endregion 


        #region Methods
        private void OnClickClear() => _console.ClearConsole();
        private void OnClickClose() => _consoleContainer.Visible = false;
        private void OnInputTextSubmited(string cmd) => HandleEntry(cmd);

        private void OnClickSend()
        {
            string command = _input.Text;
            HandleEntry(command);
        }

        private void OnConsoleEntryAdded(string entry, ConsoleEntryType type)
        {
            Color col = GetColorFromEntryType(type);
            string hex = col.ToHtml();
            _consoleLogsText.Text += $"[color=#{hex}]{entry}[/color]\n";
        }

        private void OnTabsChanged(long tabIdx)
        {
            _loggerLogsText.Visible = tabIdx == 2;
            _consoleLogsText.Visible = tabIdx == 0;
        }

        private void OnLogEntryAdded(string msg, LogLevel lvl, string formatted)
        {
            Color col = GetColorFromLogLevel(lvl);
            string hex = col.ToHtml();
            _loggerLogsText.Text += $"[color=#{hex}]{formatted}[/color]\n";
        }
        #endregion


        #region Helper Methods
        private void ClearLogText()
        {
            _consoleLogsText.Text = "";
        }

        private void HandleEntry(string entry)
        {
            _input.Text = "";
            _console.HandleLogInputCommand(entry); 
        }

        private Color GetColorFromEntryType(ConsoleEntryType type)
        {
            switch(type)
            {
                case ConsoleEntryType.ConsoleMessage:
                return Colors.AntiqueWhite;

                case ConsoleEntryType.UserInput:
                return Colors.LightBlue;

                case ConsoleEntryType.Error:
                return Colors.OrangeRed;

                case ConsoleEntryType.Warning:
                return Colors.Orange;

                default:
                return Colors.LightBlue;
            }
        }

        private Color GetColorFromLogLevel(LogLevel type)
        {
            switch(type)
            {
                case LogLevel.ALL:
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                return Colors.AntiqueWhite;

                case LogLevel.ERROR:
                case LogLevel.FATAL:
                return Colors.OrangeRed;

                case LogLevel.WARNING:
                return Colors.OrangeRed;

                default:
                return Colors.AntiqueWhite;
            }
        }
        #endregion
    }
}