using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Text.Json;
using BSXIV.FFXIV.Lodestone;
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
using StackExchange.Redis;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BSXIV
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private CommandHandler _commands;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Version AppVersion;

        public static JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        
        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
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
                .AddSingleton<LodestoneRequester>()
                .AddSingleton(_ => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS")!))
                .AddSingleton(provider => new CharacterProcessor(provider.GetRequiredService<LoggingUtils>(), provider.GetRequiredService<LodestoneRequester>(), provider.GetRequiredService<DbContext>(), provider.GetRequiredService<ConnectionMultiplexer>()))
                .BuildServiceProvider();
        }
    }    
}