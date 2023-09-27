using System;

namespace INUlib.BackendToolkit.SpreadSheets
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CSVColumnAttribute : Attribute
    {
        #region Fields
        public readonly string name;
        public readonly bool important;
        #endregion Fields

        #region Constructors
        public CSVColumnAttribute()
        {
            this.name = null;
            important = false;
        }
        
        public CSVColumnAttribute(string name)
        {
            this.name = name;
            important = false;
        }

        public CSVColumnAttribute(bool important)
        {
            this.important = important;
        }

        public CSVColumnAttribute(string name, bool important)
        {
            this.name = name;
            this.important = important;
        }
        #endregion Constructors
    }
}