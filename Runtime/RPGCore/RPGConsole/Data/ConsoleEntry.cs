namespace RPGCore.RPGConsole.Data
{
    public class ConsoleEntry
    {
        public readonly string Description;
        public readonly ConsoleEntryType EntryType;

        public ConsoleEntry(string description, ConsoleEntryType entryType)
        {
            Description = description;
            EntryType = entryType;
        }
    }
}