#if TOOLS
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
        [Export] private RichTextLabel _logsText;
        #endregion

        #region Fields
        private CheatConsole _console;
        #endregion


        #region Godot Methods
        public override void _Ready()
        {
            _input.Text = "";
            _logsText.Text = "";
            if(_sendBtn != null) _sendBtn.Pressed += OnClickSend;
            if(_closeBtn != null) _closeBtn.Pressed += OnClickClose;
            if(_clearBtn != null) _clearBtn.Pressed += OnClickClear;
            if(_input != null) _input.TextSubmitted += OnInputTextSubmited;

            _console = new CheatConsole(256);
            _console.onEntryAddedToLog += OnLogEntryAdded;
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

        private void OnLogEntryAdded(string entry, ConsoleEntryType type)
        {
            Color col = GetColorFromEntryType(type);
            string hex = col.ToHtml();
            _logsText.Text += $"[color=#{hex}]{entry}[/color]\n";
        }
        #endregion


        #region Helper Methods
        private void ClearLogText()
        {
            _logsText.Text = "";
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
        #endregion
    }
}
#endif