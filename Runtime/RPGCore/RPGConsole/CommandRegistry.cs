using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using System.Diagnostics;
using RPGCore.RPGConsole.Data;
using System.Collections.Generic;
using RPGCore.RPGConsole.Commands;
using RPGCore.RPGConsole.Commands.BuiltinCommands;

namespace RPGCore.RPGConsole
{
    public class CommandRegistry
    {
        #region Static Fields
        private static readonly UnityEvent<CommandsContainer> OnCommandRegistered;
        #endregion Static Fields
        
        #region Fields
        private ZynithConsole m_zynithConsole;
        #endregion Fields

        
        #region Constructor
        static CommandRegistry()
        {
            OnCommandRegistered = new UnityEvent<CommandsContainer>();
        }
        
        public CommandRegistry(ZynithConsole console)
        {
            m_zynithConsole = console;
            OnCommandRegistered.AddListener(RegisterContainerCommands);
        }

        ~CommandRegistry()
        {
            OnCommandRegistered.RemoveListener(RegisterContainerCommands);
        }
        #endregion Constructor

        
        #region Static Metods
        public static void RegisterContainer(CommandsContainer commandsContainer)
        {
            OnCommandRegistered.Invoke(commandsContainer);
        }
        #endregion Static Methods
        

        #region Methods
        private void RegisterContainerCommands(CommandsContainer container)
        {
            Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
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
                ConsoleCommand data = new ConsoleCommand(container, validCommand);

                if (m_zynithConsole.ConsoleCommands.ContainsKey(data.Id))
                {
                    m_zynithConsole.AddEntryToLog($"Id Collision for {data.Id}. Latest Command will be ignored.",
                        ConsoleEntryType.ConsoleMessage);
                    continue;
                }

                validCommands++;
                m_zynithConsole.ConsoleCommands.Add(data.Id, data);
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
                    ConsoleCommand data = new ConsoleCommand(instance, validMethod);
                    
                    if (m_zynithConsole.ConsoleCommands.ContainsKey(data.Id))
                    {
                        m_zynithConsole.AddEntryToLog($"Id Collision for {data.Id}. Latest Command will be ignored.", ConsoleEntryType.ConsoleMessage);
                        continue;
                    }

                    amountOfRegisteredCommands++;
                    m_zynithConsole.ConsoleCommands.Add(data.Id, data);
                }
            }
            
            stopWatch.Stop();
            string timeInSeconds = stopWatch.Elapsed.ToString("ss\\.ff");
            
            if(amountOfRegisteredCommands > 0)
                m_zynithConsole.AddEntryToLog($"Found {amountOfRegisteredCommands} commands without dependencies" +
                                              $" and automatically added them in {timeInSeconds} seconds.", ConsoleEntryType.ConsoleMessage);
            
        }
        #endregion Methods
    }
}