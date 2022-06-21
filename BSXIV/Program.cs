using System;
using System.Reflection;
using System.Text;
using BSXIV.Utilities;
using Discord;
using Discord.WebSocket;
using BSXIV.Utilities;
using LogSeverity = BSXIV.Utilities.LogSeverity;
using DLogSeverity = Discord.LogSeverity;

namespace BSXIV
{
    public class Program
    {
        private DiscordSocketClient _client;
        private LoggingUtils _logging;
        
        public static Version AppVersion;

        public Program(LoggingUtils logging)
        {
            _logging = logging;
        }
        
        public static Task Main(string[] args) => new Program(new LoggingUtils()).MainAsync();

        private async Task MainAsync()
        {
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
            
            var exeAsm = Assembly.GetExecutingAssembly();
            AppVersion = exeAsm.GetName().Version ?? new Version(0, 0, 0);
            
            _client = new();
            _client.Log += (msg) =>
            {
                var severity = msg.Severity switch
                {
                    DLogSeverity.Debug => LogSeverity.Debug,
                    DLogSeverity.Info => LogSeverity.Info,
                    DLogSeverity.Warning => LogSeverity.Warning,
                    DLogSeverity.Error => LogSeverity.Error,
                    DLogSeverity.Critical => LogSeverity.Critical,
                    _ => LogSeverity.Info
                };
                return _logging.Log(severity, $"[{msg.Source}] {msg.Message}");
            };

            await _logging.Log(LogSeverity.Info, "===============================");
            await _logging.Log(LogSeverity.Info, $"Starting up {Constants.AppName} on version {AppVersion}");
            await _logging.Log(LogSeverity.Info, "===============================");
            await _logging.Log(LogSeverity.Info, "Submit any issues to");
            await _logging.Log(LogSeverity.Info, $"{Constants.AppWebsite}");
            await _logging.Log(LogSeverity.Info, "===============================");
            await _logging.Log(LogSeverity.Info, $"{Constants.AppCopyright}");
            await _logging.Log(LogSeverity.Info, "===============================");

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TOKEN"));
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }
    }    
}