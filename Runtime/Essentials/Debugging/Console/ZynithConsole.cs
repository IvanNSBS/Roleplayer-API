using Lib.Utils.Extensions;
using System.Collections.Generic;
using Essentials.Debugging.Console.Data;
using Essentials.Debugging.Console.View.Console;
using Essentials.Debugging.Settings;

namespace Essentials.Debugging.Console
{
    public class ZynithConsole
    {
        #region Fields
        private DebugSettings m_debugSettings;
        private Queue<string> m_consoleLogEntries;
        private Dictionary<string, ConsoleCommand> m_consoleCommands;
        private CommandRegistry m_commandRegistry;
        private IConsoleView m_consoleView;
        #endregion Fields
        
        #region Properties
        public Dictionary<string, ConsoleCommand> ConsoleCommands => m_consoleCommands;
        public IConsoleView View => m_consoleView;
        #endregion Properties
        
        
        #region Constructor
        public ZynithConsole(DebugSettings debugSettings, IConsoleView consoleView)
        {
            m_consoleLogEntries = new Queue<string>();
            m_consoleCommands = new Dictionary<string, ConsoleCommand>();
            m_commandRegistry = new CommandRegistry(this);
            
            m_consoleView = consoleView;
            m_debugSettings = debugSettings;
            
            m_commandRegistry.InitializeZynithCommands();
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
            
            string[] split = commandString .Split(' ');
            
            string commandId = split[0];
            string[] commandArgs = split.SubArray(1);
            
            AddEntryToLog(commandString, ConsoleEntryType.UserInput);
            
            if (m_consoleCommands.ContainsKey(commandId))
            {
                var command = m_consoleCommands[split[0]];
                ConsoleEntry invokeMessage = CommandHandler.InvokeCommand(command, commandArgs, out bool argumentsWereHandled);

                if(invokeMessage != null)
                    AddEntryToLog(invokeMessage.Description, invokeMessage.EntryType);
            }
            else
                AddEntryToLog($"Unknown Command: {commandId}", ConsoleEntryType.Error);

            m_consoleView.OnEntrySubmitted();
        }

        public void ClearConsole()
        {
            m_consoleLogEntries.Clear();
            m_consoleView.ConsoleCleared();
        }
        #endregion Methods
    }
}