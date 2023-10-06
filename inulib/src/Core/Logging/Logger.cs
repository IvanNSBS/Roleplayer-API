using System;

namespace INUlib.Core
{
    public enum LogLevel
    {
        ALL = -1,
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        FATAL = 4,
        OFF = 5
    }

    public partial class Logger
    {
        #region Private Singleton
        private static Logger _instance;
        private static Logger instance
        {
            get
            {
                if(_instance == null)
                    _instance = new Logger();
                
                return _instance;
            }
        }
        #endregion


        #region Fields
        /// <summary>
        /// Delegate for OnLogReceived
        /// </summary>
        /// <param name="msg">The message given for logging</param>
        /// <param name="level">The log level for the debug</param>
        /// <param name="formattedMsg">
        /// The formatted log message, with the
        /// [HH:MM:SS] LOG_LEVEL: msg format
        /// </param>
        public delegate void LogReceived(string msg, LogLevel level, string formattedMsg);
        public static event LogReceived onLogReceived = delegate { };
        private static LogLevel _acceptedLogLevel = LogLevel.DEBUG;
        #endregion


        #region Partial Methods
        partial void InfoLog(string msg);
        partial void DebugLog(string msg);
        partial void WarningLog(string msg);
        partial void ErrorLog(string msg);
        partial void FatalLog(string msg);
        #endregion


        #region Methods
        public static void SetLogLevel(LogLevel level) => _acceptedLogLevel = level;

        private static void Log(string msg, LogLevel logLevel)
        {
            if((int)logLevel < (int)_acceptedLogLevel)
                return;

            switch(logLevel)
            {
                case LogLevel.DEBUG:
                    instance.DebugLog(msg);
                    break;
                case LogLevel.WARNING:
                    instance.WarningLog(msg);
                    break;
                case LogLevel.ERROR:
                    instance.ErrorLog(msg);
                    break;
                default:
                    instance.DebugLog(msg);
                    break;
            }

            var now = DateTime.Now;
            string hour = now.Hour.ToString("00");
            string minute = now.Minute.ToString("00");
            string second = now.Second.ToString("00");
            string formattedMsg = $"[{hour}:{minute}:{second}] {logLevel}: {msg}";

            onLogReceived?.Invoke(msg, logLevel, formattedMsg);
        }

        public static void Debug(string msg) => Log(msg, LogLevel.DEBUG);
        public static void Info(string msg) => Log(msg, LogLevel.INFO);
        public static void Warning(string msg) => Log(msg, LogLevel.WARNING);
        public static void Error(string msg) => Log(msg, LogLevel.ERROR);
        public static void Fatal(string msg) => Log(msg, LogLevel.FATAL);
        #endregion
    }
}