using UnityEngine;
using Essentials.Debugging.Loggers.Interfaces;
using Essentials.Debugging.Settings;

namespace Essentials.Debugging.Loggers
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
        public delegate void LogMessageReceived(string str, LogLevels lvl);
        public static event LogMessageReceived logMessageReceived;
        #endregion Events


        #region Constructors
        private ZynithLogger(DebugSettings settings)
        {
            m_policy = new FilePolicy(settings);
        }

        ~ZynithLogger()
        {
            Application.logMessageReceived -= AdaptLogCallback;
            Application.quitting -= OnQuit;
        }
        #endregion Constructors


        #region Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (s_instance == null)
            {
                s_instance = new ZynithLogger(DebugSettings.GetDebugSettings());
                Application.logMessageReceived += AdaptLogCallback;
                Application.quitting += OnQuit;
            }
        }

        private static void OnQuit()
        {
            s_instance = null;
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
                    stackTrace = stackTrace.Substring(0, stackTrace.Length - 2);
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
        
        private static void Log(LogLevels level, string msg, bool fromUnityCallback = false)
        {
            string formattedMsg = s_instance.m_policy.Log(level, msg, fromUnityCallback);
            logMessageReceived?.Invoke(formattedMsg, level);
        }
        #endregion Methods
    }
}