using System.Collections.Generic;
using System.Linq;
using INUlib.Gameplay.Debugging.Console.Data;
using INUlib.Gameplay.Debugging.Console.View.Debugger;

namespace INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands
{
    public class ConsoleHelperCommandsContainer : CommandsContainer
    {
        private CheatConsole m_console;
        private DebuggerView m_debuggerView;
        
        public ConsoleHelperCommandsContainer(CheatConsole console, DebuggerView debuggerView)
        {
            m_console = console;
            m_debuggerView = debuggerView;
        }
        
        [ConsoleCommand("close", "Close the Debugger view")]
        public void CloseConsole()
        {
            m_debuggerView.CloseDebugger();
        }
        
        [ConsoleCommand("clearConsole", "Clears every message logged to Debugger Console")]
        public void ClearConsole()
        {
            m_console.ClearConsole();
        }
        
        [ConsoleCommand("clearLogger", "Clears every message logged to Debugger Logger View. Log file will not be cleared")]
        public void ClearLogger()
        {
            m_debuggerView.LoggerView.ClearLog();
        }

        [ConsoleCommand("help", "Prints every command alias registered to the Console")]
        public void ShowAllCommands()
        {
            foreach (var commandsWithSameId in m_console.ConsoleCommands)
            {
                string entry = $"{commandsWithSameId.Key} [{commandsWithSameId.Value.Count()} available signatures]";
                m_console.AddEntryToLog(entry, ConsoleEntryType.ConsoleMessage);
            }
        }

        [ConsoleCommand("help", "Show the usage of a command")]
        public ConsoleEntry GetCommandUsage(string commandId)
        {
            if (!m_console.ConsoleCommands.ContainsKey(commandId))
                return new ConsoleEntry($"Command <{commandId}> does not exist", ConsoleEntryType.Warning);

            string result = $"Registered Signatures for {commandId}: \n";

            List<string> signatures = new List<string>();
            foreach (var command in m_console.ConsoleCommands[commandId])
                signatures.Add($"{command.GetNamedSignature()} - {command.Description}");

            return new ConsoleEntry(result + string.Join("\n", signatures), ConsoleEntryType.ConsoleMessage);
        }
    }
}