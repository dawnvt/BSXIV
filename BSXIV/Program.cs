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
using NLog.Extensions.Logging;
using StackExchange.Redis;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BSXIV
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private CommandHandler _commands;
        private ILogger _logger;

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
            _services = ConfigureServices();
            _logger = _services.GetRequiredService<ILogger<Program>>();
            _logger.LogInformation("setting up program!");

            var exeAsm = Assembly.GetExecutingAssembly();
            AppVersion = exeAsm.GetName().Version ?? new Version(0, 0, 0);

            
            _client = _services.GetRequiredService<DiscordSocketClient>();

            _client.Log += message =>
            {
                switch(message.Severity)
                {
                    case LogSeverity.Critical:
                        _logger.LogCritical(message.Exception, message.Message);
                        break;
                    case LogSeverity.Error:
                        _logger.LogError(message.Exception, message.Message);
                        break;
                    case LogSeverity.Warning:
                        _logger.LogWarning(message.Exception, message.Message);
                        break;
                    case LogSeverity.Info:
                        _logger.LogInformation(message.Exception, message.Message);
                        break;
                    case LogSeverity.Verbose:
                        _logger.LogTrace(message.Exception, message.Message);
                        break;
                    case LogSeverity.Debug:
                        _logger.LogDebug(message.Exception, message.Message);
                        break;
                };
                return Task.CompletedTask;
            };

            _client.Ready += async () =>
            {
                await _services.GetRequiredService<CommandHandler>().InitializeAsync();
            };
            
            // Do not touch anything below this line unless you absolutely have to.
            var token = Environment.GetEnvironmentVariable("TOKEN");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            _logger.LogInformation("Ready!");
            
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
                .AddLogging(log =>
                {
                    log.ClearProviders();
                    log.SetMinimumLevel(LogLevel.Trace);
                    log.AddNLog(logConfig);
                })
                .AddSingleton(new DiscordSocketClient(disConfig))
                .AddInstancedSingleton<InteractionService>()
                .AddInstancedSingleton<CommandHandler>()
                .AddInstancedSingleton<DbContext>()
                .AddInstancedSingleton<WebRequest>()
                .AddInstancedSingleton<LodestoneRequester>()
                .AddSingleton(_ => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS")!))
                .AddInstancedSingleton<CharacterProcessor>()
                .BuildServiceProvider();
        }
    }

    public static class Extension
    {
        public static IServiceCollection AddInstancedSingleton<T>(this IServiceCollection collection) where T : class
        {
            return collection.AddSingleton(provider => ActivatorUtilities.CreateInstance<T>(provider));
        }
    }
}