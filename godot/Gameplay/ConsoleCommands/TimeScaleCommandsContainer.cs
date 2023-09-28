using Godot;
using System.Globalization;

namespace INUlib.Gameplay.Debugging.Console.Commands.BuiltinCommands
{
    public class TimeScaleCommandsContainer : CommandsContainer
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