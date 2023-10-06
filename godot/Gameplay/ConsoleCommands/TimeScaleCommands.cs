using System.Globalization;
using INUlib.Gameplay.Debugging.Console;

namespace Godot.INUlib.Gameplay.Commands
{
    public class TimeScaleCommands : CommandsContainer
    {

        [ConsoleCommand("timescale", "Sets the TimeScale to the given value")]
        public string SetTimeScale(float value)
        {
            Engine.TimeScale = value;
            return $"Timescale is now: {Engine.TimeScale.ToString(CultureInfo.InvariantCulture)}";
        }
        
        [ConsoleCommand("timescale", "Prints the current TimeScale")]
        public string GetTimeScale()
        {
            return $"Timescale: {Engine.TimeScale.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}