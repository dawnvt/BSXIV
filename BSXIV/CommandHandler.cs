using System.Reflection;
using BSXIV.Utilities;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BSXIV
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private InteractionService _interaction;
        private IServiceProvider _services;
        
        public CommandHandler(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _interaction = services.GetRequiredService<InteractionService>();
            _services = services;

            _client.SlashCommandExecuted += SlashCommand;
        }

        private async Task SlashCommand(SocketSlashCommand args)
        {
            var result = await _interaction.ExecuteCommandAsync(
                new SocketInteractionContext<SocketSlashCommand>(_client, args),
                _services);

            if (result.Error != null)
            {
                // await _log.Log(LogSeverity.Error, result.ErrorReason);
            }
        }

        public async Task InitializeAsync()
        {
            var modules = await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            foreach (var servers in _client.Guilds)
            {
                if (modules.ToArray() == null)
                {
                    // await _log.Log(LogSeverity.Error, "'modules.ToString()' is null! Commands won't show up!");
                }
                await _interaction.AddModulesToGuildAsync(servers, true, modules.ToArray());
            }
        }
    }
}