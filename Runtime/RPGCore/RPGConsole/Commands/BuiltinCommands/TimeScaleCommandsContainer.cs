using System.Globalization;
using RPGCore.RPGConsole.Data;
using UnityEngine;

namespace RPGCore.RPGConsole.Commands.BuiltinCommands
{
    public class TimeScaleCommandsContainer : CommandsContainer
    {
        public TimeScaleCommandsContainer() : base()
        {
        }

        [ConsoleCommand("timescale.set", "Sets the TimeScale")]
        public string SetTimeScale(float value)
        {
            Time.timeScale = value;
            ConsoleEntry msg = new ConsoleEntry("", ConsoleEntryType.ConsoleMessage);
            return $"Timescale is now: {Time.timeScale.ToString(CultureInfo.InvariantCulture)}";
        }
        
        [ConsoleCommand("timescale.get", "Gets the current TimeScale")]
        public string GetTimeScale()
        {
            return $"Timescale: {Time.timeScale.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}