using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Essentials.Debugging.Console.Commands;
using Essentials.Debugging.Console.Data;

namespace Essentials.Debugging.Console
{
    public class CommandRegistry
    {
        #region Static Fields
        private static Queue<CommandsContainer> s_registeredContainers = new Queue<CommandsContainer>();
        private delegate void RegisterCommand(CommandsContainer container);
        private static event RegisterCommand OnCommandRegistered;
        #endregion Static Fields
        
        #region Fields
        private ZynithConsole m_zynithConsole;
        #endregion Fields

        
        #region Constructor
        public CommandRegistry(ZynithConsole console)
        {
            m_zynithConsole = console;

            for(int i = s_registeredContainers.Count()-1; i >= 0; i--)
                RegisterContainerCommands(s_registeredContainers.Dequeue());
            
            s_registeredContainers.Clear();
            OnCommandRegistered += RegisterContainerCommands;
        }

        ~CommandRegistry()
        {
            OnCommandRegistered -= RegisterContainerCommands;
        }
        #endregion Constructor

        
        #region Static Metods
        public static void RegisterContainer(CommandsContainer commandsContainer)
        {
            s_registeredContainers.Enqueue(commandsContainer);
            OnCommandRegistered?.Invoke(commandsContainer);
        }
        
        public static string GetMethodSignature(MethodInfo methodInfo)
        {
            List<string> alias = new List<string>();
            
            foreach (var param in methodInfo.GetParameters())
                alias.Add(TypeAlias.GetPrimitiveTypeAlias(param.ParameterType));

            return string.Join(" ", alias);
        }
        #endregion Static Methods
        

        #region Methods
        private void RegisterContainerCommands(CommandsContainer container)
        {
            BindingFlags validMethodsFlags = BindingFlags.Public | BindingFlags.Instance;

            var commandClassType = container.GetType();

            var methodsWithCommandAttribute = commandClassType.GetMethods(validMethodsFlags)
                .Where(method => method.GetCustomAttribute<ConsoleCommandAttribute>() != null);

            var validCommandMethods = methodsWithCommandAttribute.Where(method =>
                method.GetParameters().All(param =>
                    param.ParameterType.IsPrimitive || param.ParameterType == typeof(string)));

            int validCommands = 0;
            foreach (var validCommand in validCommandMethods)
            {
                ConsoleCommand consoleCommand = new ConsoleCommand(container, validCommand);
                
                if(ProcessCommand(consoleCommand))
                    validCommands++;
            }

            string name = container.GetType().Name.Replace("Container", "");
            m_zynithConsole.AddEntryToLog($"Registered {validCommands} commands for container {name}.", ConsoleEntryType.ConsoleMessage);
        }
        
        public void InitializeZynithCommands()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var type = typeof(CommandsContainer);
            Type[] commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract).ToArray();

            BindingFlags validMethodsFlags = BindingFlags.Public | BindingFlags.Instance;

            int amountOfRegisteredCommands = 0;
            
            foreach (var commandClassType in commandTypes)
            {
                var constructors = commandClassType.GetConstructors();
                
                bool validConstructorSignature = constructors.Count() == 1 && !constructors[0].GetParameters().Any();

                if (!validConstructorSignature) continue;

                var methodsWithCommandAttribute = commandClassType.GetMethods(validMethodsFlags)
                    .Where(method => method.GetCustomAttribute<ConsoleCommandAttribute>() != null);
                
                var validCommandMethods = methodsWithCommandAttribute.
                    Where(method => method.GetParameters().All(param => param.ParameterType.IsPrimitive || param.ParameterType == typeof(string)));
                
                foreach (var validMethod in validCommandMethods)
                {
                    var instance = (CommandsContainer)Activator.CreateInstance(commandClassType);
                    ConsoleCommand consoleCommand = new ConsoleCommand(instance, validMethod);

                    if(ProcessCommand(consoleCommand))
                        amountOfRegisteredCommands++;
                }
            }
            
            stopWatch.Stop();
            string timeInSeconds = stopWatch.Elapsed.ToString("ss\\.ff");
            
            if(amountOfRegisteredCommands > 0)
                m_zynithConsole.AddEntryToLog($"Found {amountOfRegisteredCommands} commands without dependencies" +
                                              $" and automatically added them in {timeInSeconds} seconds.", ConsoleEntryType.ConsoleMessage);
            
        }
        #endregion Methods
        
        
        #region Helper Methods
        /// <summary>
        /// Process a console command and try to add it to the available console commands
        /// </summary>
        /// <param name="command">The command to be added</param>
        /// <returns>True if the command was added to the list of console commands. False Otherwise</returns>
        private bool ProcessCommand(ConsoleCommand command)
        { 
            if (!m_zynithConsole.ConsoleCommands.ContainsKey(command.Id))
            {
                m_zynithConsole.ConsoleCommands.Add(command.Id, new List<ConsoleCommand>{ command });
                return true;
            }

            var commandsWithSameAlias = m_zynithConsole.ConsoleCommands[command.Id];
            var newCommandSignature = GetMethodSignature(command.CommandMethod);

            // no command with same signature
            if (commandsWithSameAlias.All(x => GetMethodSignature(x.CommandMethod) != newCommandSignature))
            {
                m_zynithConsole.ConsoleCommands[command.Id].Add(command);
                return true;
            }
            
            m_zynithConsole.AddEntryToLog($"Command Signature Collision with Id: {command.Id}. " +
                                          $"Latest Command will be ignored.", ConsoleEntryType.ConsoleMessage);

            return false;
        }
        #endregion Helper Methods
    }
}