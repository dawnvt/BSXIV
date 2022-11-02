using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Text.Json;
using BSXIV.Utilities;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using DLogSeverity = Discord.LogSeverity;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BSXIV
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private CommandHandler _commands;

        public static Version AppVersion;

        public static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        
        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                /* ignored due to architecture */
                logger.Info("Hello!");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped due to uncaught exception!");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
            
            var exeAsm = Assembly.GetExecutingAssembly();
            AppVersion = exeAsm.GetName().Version ?? new Version(0, 0, 0);

            _services = ConfigureServices();
            
            _client = _services.GetRequiredService<DiscordSocketClient>();


            _client.Ready += async () =>
            {
                await _services.GetRequiredService<CommandHandler>().InitializeAsync();
            };

            // Do not touch anything below this line unless you absolutely have to.
            var token = Environment.GetEnvironmentVariable("TOKEN");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }
        
        private IServiceProvider ConfigureServices()
        {
            var logConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            var disConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(disConfig))
                .AddSingleton(provider => new InteractionService(provider.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .AddSingleton<DbContext>()
                .AddLogging(log =>
                {
                    log.ClearProviders();
                    log.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    log.AddNLog(logConfig);
                })
                .AddSingleton<WebRequest>(provider => new WebRequest())
                .BuildServiceProvider();
        }

        private enum LogSeverity
        {
            None = LogLevel.None,
            Trace = LogLevel.Trace,
            Info = LogLevel.Information,
            Debug = LogLevel.Debug,
            Warning = LogLevel.Warning,
            Error = LogLevel.Error,
            Critical = LogLevel.Critical
        }
    }    
}