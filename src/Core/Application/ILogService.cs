using System;

namespace Core.Application
{
    public interface ILogService
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);

        void Error(string message);
        void Error(string message, Exception exception);
        void Error(Exception exception);
    }
}
