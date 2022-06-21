using System.Diagnostics;
using System.Reflection;

namespace BSXIV.Utilities
{
    /*
     * Logging format goes as following
     * Time of logging (UTC)    Class     Severity Source  Message
     * [18/06/2022 10:20:12 PM] [Program] [Info] [Gateway] Startup finished.
     */

    public enum LogSeverity
    {
        Debug = 0,
        Info,
        Warning,
        Error, 
        Critical
    }

    public class LoggingUtils
    {
        public Task Log(LogSeverity severity, string msg)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.WriteLine($"[{DateTime.UtcNow}] [{NameOfCallingClass()}] [{severity.ToString()}] {msg}");
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;
        }

        private static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }
    }
}