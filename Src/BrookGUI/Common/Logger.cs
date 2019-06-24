using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Brook.MainWin.Utils
{
    public class Logger : IDisposable
    {
        private static readonly string logName = $"Brook_{Helpers.StartUpTimeStampAsFileName}.log";
        public static readonly string AppLogFile = Path.Combine(Helpers.ApplicationDirectory, Logger.logName);
        private static readonly Logger instance = new Logger();
        private StreamWriter w;

        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }

        private Logger()
        {
            w = File.AppendText(AppLogFile);
            w.AutoFlush = true;
            Log("Start", "=== Logging started ===");
        }

        ~Logger()
        {
            if (w != null)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            Log("_Stop", "=== Logging stopped ===");
            w.Flush();
            w.Close();
            w = null;
        }

        public void Trace(string logMessage)
        {
            Log("Trace", logMessage);
        }

        public void Info(string logMessage)
        {
            Log("_Info", logMessage);
        }

        public void Debug(string logMessage)
        {
            Log("Debug", logMessage);
        }

        public void Perf(string logMessage)
        {
            Log("_Perf", logMessage);
        }

        public void Error(string logMessage)
        {
            Log("Error", logMessage);
            FlushIt();
        }

        public void Error(Exception e, string logMessage = "", [CallerMemberName] string memberName = "")
        {
            Trace($"Exception encountered in:  {memberName}");
            var text = e.AsLoggableString();
            logMessage = ((!string.IsNullOrEmpty(logMessage))
                ? ($"{logMessage}{Environment.NewLine}{text}")
                : text);
            Error(logMessage);
        }

        public void FlushIt()
        {
            w.Flush();
        }

        public void TraceEntry([CallerMemberName] string memberName = "")
        {
            Trace($">>>  {memberName}");
        }

        public void TraceExit([CallerMemberName] string memberName = "")
        {
            Trace($"<<<  {memberName}");
        }

        private void Log(string logLevel, string logMessage)
        {
            string logThis = $"[{Helpers.CurrentTimeForLogging}] : [{logLevel}] :  {logMessage}";
            w.WriteLine(logThis);
            Console.WriteLine(logThis);
        }
    }
}
