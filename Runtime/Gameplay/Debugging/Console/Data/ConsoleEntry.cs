namespace INUlib.Gameplay.Debugging.Console.Data
{
    /// <summary>
    /// ConsoleEntry is what commands should return if they wish to
    /// add a message to the console.
    /// A string can also be used instead of ConsoleEntry
    /// </summary>
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