using INUlib.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using INUlib.Gameplay.Debugging.Console.Data;
using System;
using INUlib.Core;
using INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands;

namespace INUlib.Gameplay.Debugging.Console
{
    public class CheatConsole
    {
        #region Fields
        private uint _logBufferSize;
        private Queue<string> m_consoleLogEntries;
        private Dictionary<string, List<ConsoleCommand>> m_consoleCommands;
        private CommandRegistry m_commandRegistry;
        #endregion Fields
        
        #region Properties
        public Dictionary<string, List<ConsoleCommand>> ConsoleCommands => m_consoleCommands;
        #endregion Properties
        

        #region Events
        public Action<string, ConsoleEntryType> onEntryAddedToLog = delegate { };
        public Action onConsoleCleared = delegate { };
        #endregion    


        #region Constructor
        public CheatConsole(uint logBufferSize)
        {
            _logBufferSize = logBufferSize;
            m_consoleLogEntries = new Queue<string>();
            m_consoleCommands = new Dictionary<string, List<ConsoleCommand>>();
            m_commandRegistry = new CommandRegistry(this);
        }
        #endregion Constructor
        
        
        #region Methods
        /// <summary>
        /// Initializes the console, automatically registering every command
        /// that has no dependencies
        /// </summary>
        public void Init() 
        {
            m_commandRegistry.InitializeConsoleCommands();
            CommandRegistry.RegisterContainer(new ConsoleHelperCommands(this));
            AddEntryToLog("Type help to see all available commands", ConsoleEntryType.ConsoleMessage);
            AddEntryToLog("Type help cmd to see usages of a command", ConsoleEntryType.ConsoleMessage);
        } 

        public void AddEntryToLog(string logEntry, ConsoleEntryType entryType)
        {
            onEntryAddedToLog?.Invoke(logEntry, entryType);
            m_consoleLogEntries.Enqueue(logEntry);
            if (m_consoleLogEntries.Count > _logBufferSize)
                m_consoleLogEntries.Dequeue();
        }
        
        /// <summary>
        /// Handles the command in the console inputString. Prints if the command is valid or not
        /// Also appends the command to the console buffer, wrong or not.
        /// </summary>
        public void HandleLogInputCommand(string commandString)
        {
            if (commandString == "")
            {
                return;
            }
            
            string[] split = commandString.Split(' ');
            
            string commandId = split[0];
            string[] commandArgs = split.SubArray(1);
            
            AddEntryToLog(commandString, ConsoleEntryType.UserInput);
            var command = FindCommandThatMatchesArgs(commandId, commandArgs);
            
            if (command != null)
            {
                ConsoleEntry invokeMessage = CommandHandler.InvokeCommand(command, commandArgs, out bool argumentsWereHandled);
                if(invokeMessage != null)
                    AddEntryToLog(invokeMessage.Description, invokeMessage.EntryType);
            }
            else
            {
                AddEntryToLog("No command matches the given signature", ConsoleEntryType.Error);
            }
        }

        public void ClearConsole()
        {
            m_consoleLogEntries.Clear();
            onConsoleCleared?.Invoke();
        }
        #endregion Methods
        
        
        #region Utility Methods
        private ConsoleCommand FindCommandThatMatchesArgs(string commandId, string[] args)
        {
            if (!m_consoleCommands.ContainsKey(commandId))
                return null;

            foreach (var command in m_consoleCommands[commandId])
            {
                var parameters = command.CommandMethod.GetParameters();
                if(args.Length != parameters.Length)
                    continue;

                bool success = true;
                for(int i = 0; i < parameters.Length; i++)
                {
                    CommandHandler.ParseArgument(args[i], parameters[i], out bool successfull);
                    success &= successfull;
                }
                
                if (success)
                    return command;
            }

            return null;
        }
        #endregion Utility Methods
    }
}