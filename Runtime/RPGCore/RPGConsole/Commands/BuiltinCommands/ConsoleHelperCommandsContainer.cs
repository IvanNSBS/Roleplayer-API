using RPGCore.RPGConsole.Data;

namespace RPGCore.RPGConsole.Commands.BuiltinCommands
{
    public class ConsoleHelperCommandsContainer : CommandsContainer
    {
        private ZynithConsole m_zynithConsole;
        
        public ConsoleHelperCommandsContainer(ZynithConsole zynithConsole)
        {
            m_zynithConsole = zynithConsole;
        }
        
        [ConsoleCommand("close", "Close the console view")]
        public void CloseConsole()
        {
            m_zynithConsole.View.CloseConsole();
        }
        
        
        [ConsoleCommand("clear", "Clears every message logged to Zynith Console")]
        public void ClearConsole()
        {
            m_zynithConsole.ClearConsole();
        }

        [ConsoleCommand("showall", "Shows every command registered to Zynith Console")]
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