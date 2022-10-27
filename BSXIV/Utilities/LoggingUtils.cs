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
            Console.ForegroundColor = severity switch
            {
                LogSeverity.Debug => ConsoleColor.DarkMagenta,
                LogSeverity.Info => ConsoleColor.White,
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Critical => ConsoleColor.DarkRed,
                _ => Console.ForegroundColor
            };
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