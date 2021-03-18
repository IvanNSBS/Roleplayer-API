using RPGCore.RPGConsole.Data;

namespace RPGCore.RPGConsole.View.Console
{
    public interface IConsoleView
    {
        void OnEntrySubmitted();
        void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType);
        void ConsoleQueueExceeded();
        void ConsoleCleared();
    }
}