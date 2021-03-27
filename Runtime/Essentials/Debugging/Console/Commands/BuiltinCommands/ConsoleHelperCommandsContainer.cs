using System.Collections.Generic;
using System.Linq;
using Essentials.Debugging.Console.Data;
using Essentials.Debugging.Console.View.Debugger;

namespace Essentials.Debugging.Console.Commands.BuiltinCommands
{
    public class ConsoleHelperCommandsContainer : CommandsContainer
    {
        private ZynithConsole m_zynithConsole;
        private DebuggerView m_debuggerView;
        
        public ConsoleHelperCommandsContainer(ZynithConsole zynithConsole, DebuggerView debuggerView)
        {
            m_zynithConsole = zynithConsole;
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
            m_zynithConsole.ClearConsole();
        }
        
        [ConsoleCommand("clearLogger", "Clears every message logged to Debugger Logger View. Log file will not be cleared")]
        public void ClearLogger()
        {
            m_debuggerView.LoggerView.ClearLog();
        }

        [ConsoleCommand("help", "Prints every command alias registered to the Console")]
        public void ShowAllCommands()
        {
            foreach (var commandsWithSameId in m_zynithConsole.ConsoleCommands)
            {
                string entry = $"{commandsWithSameId.Key} [{commandsWithSameId.Value.Count()} available signatures]";
                m_zynithConsole.AddEntryToLog(entry, ConsoleEntryType.ConsoleMessage);
            }
        }

        [ConsoleCommand("help", "Show the usage of a command")]
        public ConsoleEntry GetCommandUsage(string commandId)
        {
            if (!m_zynithConsole.ConsoleCommands.ContainsKey(commandId))
                return new ConsoleEntry($"Command <{commandId}> does not exist", ConsoleEntryType.Warning);

            string result = $"Registered Signatures for {commandId}: \n";

            List<string> signatures = new List<string>();
            foreach (var command in m_zynithConsole.ConsoleCommands[commandId])
                signatures.Add($"{command.GetNamedSignature()} - {command.Description}");

            return new ConsoleEntry(result + string.Join("\n", signatures), ConsoleEntryType.ConsoleMessage);
        }
    }
}