using System;
using System.IO;

namespace GameManager.Services
{
    // Simple logger class that writes messages to console and a log file.
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;

            // Make sure the folder exists. If not, create it.
            string? folder = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        public void Warning(string message)
        {
            WriteLog("WARN", message);
        }

        private void WriteLog(string level, string message)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"[{time}] [{level}] {message}";

            // Write to console
            Console.WriteLine(line);

            // Write to file (but do not crash if it fails)
            try
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
            catch
            {
                // If logging to file fails, we simply ignore it.
                // We still show the message on console.
            }
        }
    }
}
