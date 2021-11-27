using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using INUlib.Common.Debugging.Console.Data;

namespace INUlib.Common.Debugging.Console
{
    public static class CommandHandler
    {
        #region Methods
        public static ConsoleEntry InvokeCommand(ConsoleCommand consoleCommand, string[] consoleArgs, out bool argumentsWereHandled)
        {
            var handledArgs = ParseArgs(consoleCommand, out bool handleSuccess, consoleArgs);
            argumentsWereHandled = handleSuccess;
            
            if (handleSuccess)
            {
                object commandResult = consoleCommand.Invoke(handledArgs);
                
                if (commandResult is string stringResult) return new ConsoleEntry(stringResult, ConsoleEntryType.ConsoleMessage);
                if (commandResult is ConsoleEntry consoleEntry) return consoleEntry;

                return null;
            }
            return new ConsoleEntry($"Correct {consoleCommand.GetCommandUsage()}", ConsoleEntryType.Warning);
        }
        
        private static object[] ParseArgs(ConsoleCommand consoleCommand, out bool success, params string[] args)
        {
            var parameters = consoleCommand.CommandMethod.GetParameters();
            // If there are no parameters for method, you cant do it wrong
            if (!parameters.Any())
            {
                success = true;
                return null;
            }

            // If there were less arguments than parameters, method won't be able to be invoked
            if (args.Count() < parameters.Count())
            {
                success = false;
                return null;
            }
            
            List<object> parsedArguments = new List<object>();
            for(int i = 0; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];
                string argument = args[i];
                object parsedArgument = ParseArgument(argument, parameter, out bool parseSuccessful);
                
                if (parseSuccessful)
                    parsedArguments.Add(parsedArgument);
                else
                {
                    // If there was a single argument that couldn't be parsed, return false;
                    success = false;
                    return null;
                }
            }

            success = true;
            return parsedArguments.ToArray();
        }
        #endregion Methods

        
        #region Utility Methods
        public static object ParseArgument(string str, ParameterInfo parameterInfo, out bool parseSuccessful)
        {
            if (parameterInfo.ParameterType == typeof(string))
            {
                parseSuccessful = true;
                return str;
            }
            if (parameterInfo.ParameterType == typeof(bool))
            {
                bool canParse = bool.TryParse(str, out bool value);
                parseSuccessful = canParse;
                return value;
            }
            if (parameterInfo.ParameterType == typeof(int))
            {
                bool canParse = int.TryParse(str, out int value);
                parseSuccessful = canParse;
                return value;
            }
            if (parameterInfo.ParameterType == typeof(float))
            {
                bool canParse = float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float value);
                parseSuccessful = canParse;
                return value;
            }

            parseSuccessful = false;
            return null;
        }
        #endregion Utility Methods
    }
}