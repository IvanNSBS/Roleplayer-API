namespace RPGCore.Loggers
{
    /// <summary>
    /// Logger wrapper that encapsulates a single log policy
    /// </summary>
    public class Logger
    {
        #region Fields
        private ILogPolicy m_policy;
        #endregion Fields

        #region Constants
        // TODO: Load constants from file
        private static readonly string LOG_FILE_PATH = "D:\\Unity Projects\\Project Small Sandbox\\Assets\\Resources\\Logs";
        private static readonly string LOG_FILE_NAME = "Log";
        #endregion Constants

        #region Singleton
        private static Logger _instance;
        public static Logger Instance 
        {   
            get
            {
                if (_instance == null)
                    _instance = new Logger(LOG_FILE_PATH, LOG_FILE_NAME);
                return _instance;
            }
        }
        #endregion Singleton


        #region Constructors
        // TODO: Find a way to create and use different log policies without changing logger class
        private Logger(string fileFolder, string fileName)
        {
            m_policy = new FilePolicy(fileFolder, fileName);
        }
        #endregion Constructors


        #region Methods
        public void Log(LogLevels level, string msg, params string[] args)
        {
            m_policy.Log(level, msg, args);
        }
        #endregion Methods
    }
}