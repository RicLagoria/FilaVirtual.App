using System;
using System.IO;

namespace FilaVirtual.App.Services
{
    public class FileLoggingService : ILoggingService
    {
        private readonly string _logPath;

        public FileLoggingService()
        {
            _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_Log.txt");
        }

        public void LogInfo(string message)
        {
            LogMessage("INFO", message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            var fullMessage = exception != null ? $"{message}\nException: {exception}" : message;
            LogMessage("ERROR", fullMessage);
        }

        public void LogDebug(string message)
        {
            LogMessage("DEBUG", message);
        }

        private void LogMessage(string level, string message)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] [{level}] {message}\n";
                File.AppendAllText(_logPath, logEntry);
            }
            catch
            {
                // Si no podemos escribir al log, no hacer nada
            }
        }
    }
}


