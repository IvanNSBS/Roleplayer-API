using System;

namespace Essentials.SpreadSheets
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CSVColumnAttribute : Attribute
    {
        public string name;

        public CSVColumnAttribute(string name)
        {
            this.name = name;
        }

        public CSVColumnAttribute()
        {
            this.name = null;
        }
    }
}