using System.Linq;
using System.Reflection;
using Essentials.Debugging.Console.Commands;

namespace Essentials.Debugging.Console.Data
{
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
        #endregion Methods
    }
}