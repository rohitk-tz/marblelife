using Core.Application;
using Core.Application.Attribute;
using NLog;
using System;

namespace Infrastructure.Application.Impl
{
    [DefaultImplementation]
    public class LogService : ILogService
    {
        private static void Log(LogLevel level, string message, Exception exception = null, string callerPath = "",
            string callerMember = "", int callerLine = 0)
        {
            var logger = LogManager.GetLogger(callerPath);
            if (!logger.IsEnabled(level)) return;
            var logEvent = new LogEventInfo(level, callerPath, message) { Exception = exception };
            
            logger.Log(logEvent);
        }
        public void Trace(string message)
        {
            Log(LogLevel.Trace, message, null, "", "", 0);
        }
        
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message, null, "", "", 0);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message, null, "", "", 0);
        }
        
        public void Error(string message)
        {
            Log(LogLevel.Error, message, null, "", "", 0);
        }

        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception, "", "", 0);
        }

        public void Error(Exception exception)
        {
            Log(LogLevel.Error, exception.Message, exception, "", "", 0);
        }
    }
}
