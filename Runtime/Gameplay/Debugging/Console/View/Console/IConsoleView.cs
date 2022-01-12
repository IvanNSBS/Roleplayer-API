using INUlib.Gameplay.Debugging.Console.Data;

namespace INUlib.Gameplay.Debugging.Console.View.Console
{
    public interface IConsoleView
    {
        void OnEntrySubmitted();
        void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType);
        void ConsoleQueueExceeded();
        void ConsoleCleared();
    }
}