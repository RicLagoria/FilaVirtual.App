using System;

namespace FilaVirtual.App.Services
{
    public interface ILoggingService
    {
        void LogInfo(string message);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message);
    }
}


