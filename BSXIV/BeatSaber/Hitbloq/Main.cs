using System.Text.Json;
using BSXIV.Utilities;
using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using SkiaSharp;

namespace BSXIV.BeatSaber.Hitbloq
{
    [RequireContext(ContextType.Guild)]
    [Group("hitbloq", "Leaderboard using the HitBlock service")]
    public class Main : InteractionModuleBase
    {
        private DbContext _dbContext;
        private WebRequest _webRequest;

        public Main(DbContext dbContext, WebRequest request)
        {
            _dbContext = dbContext;
            _webRequest = request;
        }
        [SlashCommand("adduser", "Adds your Hitbloq user")]
        private async Task AddUser(string hitbloqId)
        {
            var doc = new BsonDocument
            {
                { "hitbloqId", hitbloqId },
                { "userId", (long)Context.User.Id }
            };

            if (_dbContext.FindOne("users", new BsonDocument { { "userId", (long)Context.User.Id } }) == null)
            {
                _dbContext.Insert("users", doc);

                await RespondAsync($"User {hitbloqId} added", ephemeral: true);
            }
            else
            {
                _dbContext.Update("users", new BsonDocument { { "userId", (long)Context.User.Id } }, doc);

                await RespondAsync($"User {hitbloqId} updated", ephemeral: true);
            }
        }
    }
}