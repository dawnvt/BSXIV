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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string env = "null";

        public static Version AppVersion;

        public static JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        
        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            
            env = GatherAndLoadEnv(".env");
            _logger.Info("setting up program!");

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
            
            _logger.Info("Ready!");
            
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
                    log.SetMinimumLevel(LogLevel.Trace);
                    log.AddNLog(logConfig);
                })
                .AddSingleton<WebRequest>(provider => new WebRequest())
                .BuildServiceProvider();
        }

        private string GatherAndLoadEnv(string environment)
        {
#if DEBUG
            _logger.Info("Current environment: DEBUG");
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), environment));
            env = Path.Combine(Directory.GetCurrentDirectory(), ".env.development");
#else
            _logger.Info("Current environment: RELEASE");
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), environment));
            env = Path.Combine(Directory.GetCurrentDirectory(), ".env.production");
#endif
            return env;
        }
    }    
}