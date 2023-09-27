using System.Linq;
using System.Collections.Generic;
using INUlib.Gameplay.Debugging.Console.Data;

namespace INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands
{
    public class ConsoleHelperCommandsContainer : CommandsContainer
    {
        private CheatConsole m_console;
        
        public ConsoleHelperCommandsContainer(CheatConsole console)
        {
            m_console = console;
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