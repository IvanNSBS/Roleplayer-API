namespace RPGCore.Loggers
{
    /// <summary>
    /// Interface for Logging policise. e.g: Log to File, Log to Debug Console, etc
    /// </summary>
    public interface ILogPolicy
    {
        void Log(LogLevels level, string message, params string[] args);
    }
}