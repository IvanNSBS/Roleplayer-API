using UnityEngine;
using UnityEngine.Events;

namespace RPGCore.Loggers
{
    /// <summary>
    /// Logger wrapper that encapsulates a single log policy
    /// </summary>
    public class ZynithLogger
    {
        #region Fields
        private LogPolicy m_policy;
        #endregion Fields

        #region Singleton
        private static ZynithLogger s_instance;
        #endregion Singleton
        
        #region Events
        public static UnityEvent<string, LogLevels> logMessageReceived;
        #endregion Events


        #region Constructors
        private ZynithLogger(LoggerSettings settings)
        {
            m_policy = new FilePolicy(settings);
        }

        static ZynithLogger()
        {
            logMessageReceived = new UnityEvent<string, LogLevels>();
        }

        ~ZynithLogger()
        {
            Application.logMessageReceived -= AdaptLogCallback;
        }
        #endregion Constructors


        #region Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (s_instance == null)
            {
                s_instance = new ZynithLogger(LoggerSettings.GetLoggerSettings());
                Application.logMessageReceived += AdaptLogCallback;
            }
        }

        private static void AdaptLogCallback(string logString, string stackTrace, LogType type)
        {
            LogLevels logLevels = LogLevels.Debug;
            string message = logString;
            
            switch (type)
            {
                case LogType.Assert:
                    logLevels = LogLevels.Error;
                    break;
                case LogType.Error:
                    logLevels = LogLevels.Error;
                    break;
                case LogType.Exception:
                    logLevels = LogLevels.Exception;
                    message = $"{logString}\nStack Trace: {stackTrace}";
                    break;
                case LogType.Warning:
                    logLevels = LogLevels.Warning;
                    break;
                case LogType.Log:
                    logLevels = LogLevels.Debug;
                    break;
            }
            
            Log(logLevels, message, true);
        }
        
        public static void Log(LogLevels level, string msg, bool fromUnityCallback = false)
        {
            string formattedMsg = s_instance.m_policy.Log(level, msg, fromUnityCallback);
            logMessageReceived?.Invoke(formattedMsg, level);
        }
        #endregion Methods
    }
}