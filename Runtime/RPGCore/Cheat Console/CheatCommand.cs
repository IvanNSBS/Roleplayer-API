using System.Reflection;

namespace RPGCore.CheatConsole
{
    public class CheatCommand
    {
        #region Fields
        private readonly string m_commandId;
        private readonly string m_commandDescription;
        private readonly string m_commandFormat;
        #endregion Fields

        #region Properties
        public string CommandId => m_commandId;
        public string CommandDescription => m_commandDescription;
        public string CommandFormat => m_commandFormat;
        #endregion Properties

        #region Constructors
        protected CheatCommand(string id, string description, string format)
        {
            m_commandId = id;
            m_commandDescription = description;
            m_commandFormat = format;
        }
        #endregion Constructors
        
        
        #region Methods
        public virtual void Invoke(params string[] args) { }
        #endregion Methods
    }
}