using System.Linq;
using System.Collections.Generic;
using INUlib.Gameplay.Debugging.Console.Data;

namespace INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands
{
    public class ConsoleHelperCommands : CommandsContainer
    {
        private CheatConsole _console;
        
        public ConsoleHelperCommands(CheatConsole console)
        {
            _console = console;
        }

        [ConsoleCommand("help", "Prints every command alias registered to the Console")]
        public void ShowAllCommands()
        {
            foreach (var commandsWithSameId in _console.ConsoleCommands)
            {
                string entry = $"{commandsWithSameId.Key} [{commandsWithSameId.Value.Count()} available signatures]";
                _console.AddEntryToLog(entry, ConsoleEntryType.ConsoleMessage);
            }
        }

        [ConsoleCommand("help", "Show the usage of a command")]
        public ConsoleEntry GetCommandUsage(string commandId)
        {
            if (!_console.ConsoleCommands.ContainsKey(commandId))
                return new ConsoleEntry($"Command <{commandId}> does not exist", ConsoleEntryType.Warning);

            string result = $"Registered Signatures for {commandId}: \n";

            List<string> signatures = new List<string>();
            foreach (var command in _console.ConsoleCommands[commandId])
                signatures.Add($"{command.GetNamedSignature()} - {command.Description}");

            return new ConsoleEntry(result + string.Join("\n", signatures), ConsoleEntryType.ConsoleMessage);
        }

        [ConsoleCommand("clear", "Clear the console")]
        public void ClearConsole()
        {
            _console.ClearConsole();
        }
    }
}