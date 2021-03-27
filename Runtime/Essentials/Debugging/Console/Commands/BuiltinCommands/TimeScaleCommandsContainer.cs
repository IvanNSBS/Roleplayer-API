using UnityEngine;
using System.Globalization;

namespace Essentials.Debugging.Console.Commands.BuiltinCommands
{
    public class TimeScaleCommandsContainer : CommandsContainer
    {

        [ConsoleCommand("timescale", "Sets the TimeScale to the given value")]
        public string SetTimeScale(float value)
        {
            Time.timeScale = value;
            return $"Timescale is now: {Time.timeScale.ToString(CultureInfo.InvariantCulture)}";
        }
        
        [ConsoleCommand("timescale", "Prints the current TimeScale")]
        public string GetTimeScale()
        {
            return $"Timescale: {Time.timeScale.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}