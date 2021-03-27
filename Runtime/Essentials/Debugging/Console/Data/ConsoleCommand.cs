using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Essentials.Debugging.Console.Commands;

namespace Essentials.Debugging.Console.Data
{
    /// <summary>
    /// Console command is the class that encapsulates the methodinfo and the dependency
    /// to run a command. It's what the ZynithConsole stores to runs commands
    /// </summary>
    public class ConsoleCommand
    {
        #region Fields
        public readonly string Id;
        public readonly string Description;
        public readonly MethodInfo CommandMethod;
        private readonly CommandsContainer Instance;
        #endregion Fields

        #region Constructor
        public ConsoleCommand(CommandsContainer instance, MethodInfo commandMethod)
        {
            ConsoleCommandAttribute attribute = commandMethod.GetCustomAttribute<ConsoleCommandAttribute>();
            Id = attribute.Id;
            Description = attribute.Description;
            CommandMethod = commandMethod;
            Instance = instance;
        }
        #endregion Constructor
        
        #region Methods
        public object Invoke(object[] args)
        {
            return CommandMethod.Invoke(Instance, args);
        }
        
        /// <summary>
        /// Gets the string that shows how to use a command
        /// </summary>
        /// <returns>String representing the correct command usage</returns>
        public string GetCommandUsage()
        {
            var parameters = CommandMethod.GetParameters();

            string parameterNames = "";
            string parameterInfo = "\nParameter Info:\n";
            
            for(int i = 0; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];
                
                parameterNames += parameter.Name + " ";
                parameterInfo += $"\t - {parameter.Name}: {TypeAlias.GetPrimitiveTypeAlias(parameter.ParameterType)}";
                if (i != parameters.Count() - 1)
                    parameterInfo += "\n";
            }

            string result = "Usage: " + Id + " " + parameterNames;
            
            if (parameters.Any()) result += parameterInfo;
            
            return result;
        }

        public string GetNamedSignature()
        {
            var parameters = CommandMethod.GetParameters();
            List<string> parameterNames = new List<string>();
            for(int i = 0; i < parameters.Count(); i++)
                parameterNames.Add(parameters[i].Name);

            string signatures = string.Join(" ", parameterNames);
            
            if (string.IsNullOrEmpty(signatures))
                return Id;
            
            return $"{Id} " + signatures;
        }
        #endregion Methods
    }
}