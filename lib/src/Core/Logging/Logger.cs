using System;

namespace INUlib.Core
{
    public enum LogLevel
    {
        DEBUG = 0,
        WARNING = 1,
        ERROR = 2
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
        public static Action<string, LogLevel> onLogReceived = delegate { };
        #endregion


        #region Partial Methods
        partial void Debug(string msg);
        partial void Warning(string msg);
        partial void Error(string msg);
        #endregion


        public static void Log(string msg, LogLevel logLevel=LogLevel.DEBUG)
        {
            switch(logLevel)
            {
                case LogLevel.DEBUG:
                    instance.Debug(msg);
                    break;
                case LogLevel.WARNING:
                    instance.Warning(msg);
                    break;
                case LogLevel.ERROR:
                    instance.Error(msg);
                    break;
                default:
                    instance.Debug(msg);
                    break;
            }

            onLogReceived?.Invoke(msg, logLevel);
        }

        public static void LogError(string msg) => Log(msg, LogLevel.ERROR);
        public static void LogWarning(string msg) => Log(msg, LogLevel.WARNING);
    }
}