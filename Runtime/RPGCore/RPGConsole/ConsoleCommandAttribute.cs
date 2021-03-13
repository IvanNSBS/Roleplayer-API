using System;

namespace RPGCore.RPGConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string Id;
        public string Description;
        public ConsoleCommandAttribute(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}