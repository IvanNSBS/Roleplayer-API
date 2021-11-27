using INUlib.Common.Debugging.Console.Data;

namespace INUlib.Common.Debugging.Console.View.Console
{
    public interface IConsoleView
    {
        void OnEntrySubmitted();
        void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType);
        void ConsoleQueueExceeded();
        void ConsoleCleared();
    }
}