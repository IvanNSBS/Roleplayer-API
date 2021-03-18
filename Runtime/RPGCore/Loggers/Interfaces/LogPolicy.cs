namespace RPGCore.Loggers
{
    /// <summary>
    /// Interface for Logging policies. e.g: Log to File, Log to Debug Console, etc
    /// </summary>
    public abstract class LogPolicy
    {
        #region Fields
        protected readonly LoggerSettings loggerSettings;
        #endregion Field
        
        #region Constructor
        public LogPolicy(LoggerSettings settings)
        {
            loggerSettings = settings;
        }
        #endregion Constructor
        
        #region Methods
        /// <summary>
        /// Logs a message to the logger
        /// </summary>
        /// <param name="level">Log Level Enum</param>
        /// <param name="logEntry">Log Message</param>
        /// <param name="fromUnityCallback">Whether or not the log message came from Application.onMessageReceived</param>
        /// <returns>The Log Entry Formatted by the log policy.</returns>
        public abstract string Log(LogLevels level, string logEntry, bool fromUnityCallback = false);
        #endregion Methods
    }
}