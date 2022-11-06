using System.Reflection;
using BSXIV.Utilities;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BSXIV
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private InteractionService _interaction;
        private IServiceProvider _services;
        private ILogger _logger;
        
        public CommandHandler(DiscordSocketClient client, InteractionService interaction, ILogger<CommandHandler> logger, IServiceProvider services)
        {
            _client = client;
            _interaction = interaction;
            _services = services;

            _logger = logger;

            _client.SlashCommandExecuted += SlashCommand;
        }

        private async Task SlashCommand(SocketSlashCommand args)
        {
            var result = await _interaction.ExecuteCommandAsync(
                new SocketInteractionContext<SocketSlashCommand>(_client, args),
                _services);

            if (result.Error != null)
            { 
                _logger.LogError(result.ErrorReason);
            }
        }

        public async Task InitializeAsync()
        {
            var modules = await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            foreach (var servers in _client.Guilds)
            {
                if (modules.ToArray() == null)
                {
                    _logger.LogError("'modules.ToString()' is null! Commands won't show up!");
                }
                await _interaction.AddModulesToGuildAsync(servers, true, modules.ToArray());
            }
        }
    }
}