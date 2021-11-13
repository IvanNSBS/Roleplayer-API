using Lib.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using Essentials.Debugging.Console.Data;
using Essentials.Debugging.Console.View.Console;
using Essentials.Debugging.Settings;
using UnityEngine;

namespace Essentials.Debugging.Console
{
    public class CheatConsole
    {
        #region Fields
        private DebugSettings m_debugSettings;
        private Queue<string> m_consoleLogEntries;
        private Dictionary<string, List<ConsoleCommand>> m_consoleCommands;
        private CommandRegistry m_commandRegistry;
        private IConsoleView m_consoleView;
        #endregion Fields
        
        #region Properties
        public Dictionary<string, List<ConsoleCommand>> ConsoleCommands => m_consoleCommands;
        public IConsoleView View => m_consoleView;
        #endregion Properties
        
        
        #region Constructor
        public CheatConsole(DebugSettings debugSettings, IConsoleView consoleView)
        {
            m_consoleView = consoleView;
            m_debugSettings = debugSettings;

            m_consoleLogEntries = new Queue<string>();
            m_consoleCommands = new Dictionary<string, List<ConsoleCommand>>();
            m_commandRegistry = new CommandRegistry(this);
            
            m_commandRegistry.InitializeConsoleCommands();
        }
        #endregion Constructor
        
        
        #region Methods
        public void AddEntryToLog(string logEntry, ConsoleEntryType entryType)
        {
            m_consoleView.ConsoleEntryAdded(logEntry, entryType);
            
            m_consoleLogEntries.Enqueue(logEntry);

            if (m_consoleLogEntries.Count > m_debugSettings.LogBufferSize)
            {
                m_consoleLogEntries.Dequeue();
                m_consoleView.ConsoleQueueExceeded();
            }
        }
        
        /// <summary>
        /// Handles the command in the console inputString. Prints if the command is valid or not
        /// Also appends the command to the console buffer, wrong or not.
        /// </summary>
        public void HandleLogInputCommand(string commandString)
        {
            if (commandString == "")
            {
                m_consoleView.OnEntrySubmitted();
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
                AddEntryToLog("No command matches the given signature", ConsoleEntryType.Error);

            m_consoleView.OnEntrySubmitted();
        }

        public void ClearConsole()
        {
            m_consoleLogEntries.Clear();
            m_consoleView.ConsoleCleared();
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
                if(args.Count() != parameters.Count())
                    continue;

                bool success = true;
                for(int i = 0; i < parameters.Count(); i++)
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