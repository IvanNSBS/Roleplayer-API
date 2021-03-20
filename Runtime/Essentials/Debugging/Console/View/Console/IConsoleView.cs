using Essentials.Debugging.Console.Data;

namespace Essentials.Debugging.Console.View.Console
{
    public interface IConsoleView
    {
        void OnEntrySubmitted();
        void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType);
        void ConsoleQueueExceeded();
        void ConsoleCleared();
    }
}