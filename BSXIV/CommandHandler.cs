using System.Reflection;
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
            await _interaction.ExecuteCommandAsync(
                new SocketInteractionContext<SocketSlashCommand>(_client, args),
                _services);
        }

        public async Task InitializeAsync()
        {
            var modules = await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            foreach (var servers in _client.Guilds)
            {
                await _interaction.AddModulesToGuildAsync(servers, true, modules.ToArray());
            }
        }
    }
}