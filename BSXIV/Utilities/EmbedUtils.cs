using Discord;

namespace BSXIV.Utilities
{
    public static class EmbedUtils
    {
        public static EmbedBuilder CreateEmbed(string title, string description, uint color)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(description);
            embed.WithColor(new Color(color));
            return embed;
        }
    }
}