using Loans.Contracts;
using NLog;

namespace Loans.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) => logger.Debug(message);
        public void LogError(string message) => logger.Error(message);
        public void LogInfo(string message) => logger.Info(message);
        public void LogWarn(string message) => logger.Warn(message);
        public void LogError(string message, Exception ex) => logger.Error(ex, message);

    }
}
