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

        [ConsoleCommand("showAll", "Shows every command registered to Zynith Console")]
        public void ShowAllCommands()
        {
            foreach (var command in m_zynithConsole.ConsoleCommands)
                m_zynithConsole.AddEntryToLog($"{command.Key} - {command.Value.Description}", ConsoleEntryType.ConsoleMessage);
        }

        [ConsoleCommand("usage", "Show the usage of a command")]
        public ConsoleEntry GetCommandUsage(string commandId)
        {
            if (!m_zynithConsole.ConsoleCommands.ContainsKey(commandId))
                return new ConsoleEntry($"Command <{commandId}> don't exist", ConsoleEntryType.Warning);

            var command = m_zynithConsole.ConsoleCommands[commandId];
            return new ConsoleEntry(command.GetCommandUsage(), ConsoleEntryType.ConsoleMessage);
        }
    }
}