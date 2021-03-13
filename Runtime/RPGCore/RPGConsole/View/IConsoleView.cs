using RPGCore.RPGConsole.Data;

namespace RPGCore.RPGConsole.View
{
    public interface IConsoleView
    {
        void OpenConsole();
        void CloseConsole();
        void OnEntrySubmitted();
        void ConsoleEntryAdded(string logEntry, ConsoleEntryType entryType);
        void ConsoleEntryRemoved(string logEntry);
        void ConsoleCleared();
    }
}