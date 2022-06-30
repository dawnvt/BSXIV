using System.Text.Json;
using BSXIV.Utilities;
using Discord;
using Discord.Interactions;
using MongoDB.Bson;

namespace BSXIV.BeatSaber.Commands.ScoreSaber
{
    public class Main : InteractionModuleBase
    {
        private DbContext _dbContext;
        private WebRequest _webRequest;

        public Main(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [SlashCommand("usercount", "ScoreSaber usercount")]
        private void UserCount()
        {
            ReplyAsync(_dbContext.Find("users",new BsonDocument()).Count.ToString());
        }

        [SlashCommand("adduser", "Adds your ScoreSaber user")]
        private void AddUser(string scoreSaberId)
        {
            var doc = new BsonDocument
            {
                { "scoreSaberId", scoreSaberId },
                { "userId", (long)Context.User.Id }
            };
            
            if (_dbContext.FindOne("users", new BsonDocument{{"userId", (long)Context.User.Id}}) == null)
            {
                _dbContext.Insert("users", doc);
            }
            else
            {
                _dbContext.Update("users", new BsonDocument{{"userId", (long)Context.User.Id}}, doc);
            }
            
        }

        [SlashCommand("topscore", "Shows the top score of your ScoreSaber user")]
        private void TopScore(string scoreSaberId = "")
        {
            if (scoreSaberId == string.Empty)
            {
                if (_dbContext.FindOne("users", new BsonDocument{{"userId", (long)Context.User.Id}}) == null)
                {
                    RespondAsync("You haven't added your ScoreSaber user yet. Use `/add <scoreSaberId>` to add your user.");
                }
            }
            else
            {
                var url = $"https://scoresaber.com/api/players/{scoreSaberId}/scores?limit=1&sort=top";
                _webRequest.MakeRequestAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
        
        [SlashCommand("recentscore", "Shows the most recent score of your ScoreSaber user")]
        public void RecentScore(string scoreSaberId = "")
        {
            if (scoreSaberId == string.Empty)
            {
                if (_dbContext.FindOne("users", new BsonDocument{{"userId", (long)Context.User.Id}}) == null)
                {
                    RespondAsync("You haven't added your ScoreSaber user yet. Use `/add <scoreSaberId>` to add your user.");
                }
            }
            else
            {
                var url = $"https://scoresaber.com/api/players/{scoreSaberId}/scores?limit=1&sort=recent";
                _webRequest.MakeRequestAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        [SlashCommand("newestqualified", "Shows the most recent qualified ScoreSaber map")]
        private void NewestQualified()
        {
            var url = $"https://scoresaber.com/api/leaderboards?qualified=true&sort=0";
            _webRequest.MakeRequestAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [SlashCommand("newestranked", "Shows the most recent ranked ScoreSaber map")]
        private void NewestRanked()
        {
            var url = $"https://scoresaber.com/api/leaderboards?ranked=true&sort=0";
            _webRequest.MakeRequestAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        
        [SlashCommand("leaderboard", "Shows the leaderboard for a given ScoreSaber map")]
        private async Task ShowLeaderboard(int leaderboardId)
        {
            var url = $"https://scoresaber.com/api/leaderboards/by-id/{leaderboardId}/scores";
            var response = await _webRequest.MakeRequestAsync(url);

            if (response == null)
            {
                await RespondAsync("No leaderboard found for this map.");
            }
            else
            {
                var leaderboard = (await JsonDocument.ParseAsync(response)).Deserialize<ScoreSaberScores>();
                var embed = new EmbedBuilder
                {
                    Title = $"Leaderboard for {leaderboard?.PlayerScores[0].Leaderboard.SongName}",
                    Description = $"{leaderboard?.PlayerScores[0].Leaderboard.SongAuthorName} - {leaderboard?.PlayerScores[0].Leaderboard.SongSubName} - {leaderboard?.PlayerScores[0].Leaderboard.Difficulty.DifficultyRaw}",
                    Color = new Color(0xffde1a)
                };

                if (leaderboard?.PlayerScores != null)
                {
                    foreach (var score in leaderboard.PlayerScores)
                    {
                        embed.AddField($"{score.Score.Rank}. {score.Score.ModifiedScore}",
                            $"{score.Score.Pp}pp");
                    }
                }

                await ReplyAsync(embed: embed.Build());
            }
        }
    }
    
    public class ScoreSaberScores
    {
        public PlayerScore[] PlayerScores { get; set; }
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        public long Total { get; set; }
        public long Page { get; set; }
        public long ItemsPerPage { get; set; }
    }

    public class PlayerScore
    {
        public Score Score { get; set; }
        public Leaderboard Leaderboard { get; set; }
    }

    public class Leaderboard
    {
        public long Id { get; set; }
        public string SongHash { get; set; }
        public string SongName { get; set; }
        public string SongSubName { get; set; }
        public string SongAuthorName { get; set; }
        public string LevelAuthorName { get; set; }
        public Difficulty Difficulty { get; set; }
        public long MaxScore { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset RankedDate { get; set; }
        public DateTimeOffset QualifiedDate { get; set; }
        public dynamic LovedDate { get; set; }
        public bool Ranked { get; set; }
        public bool Qualified { get; set; }
        public bool Loved { get; set; }
        public long MaxPp { get; set; }
        public double Stars { get; set; }
        public long Plays { get; set; }
        public long DailyPlays { get; set; }
        public bool PositiveModifiers { get; set; }
        public dynamic PlayerScore { get; set; }
        public Uri CoverImage { get; set; }
        public dynamic Difficulties { get; set; }
    }

    public class Difficulty
    {
        public long LeaderboardId { get; set; }
        public long DifficultyDifficulty { get; set; }
        public string GameMode { get; set; }
        public string DifficultyRaw { get; set; }
    }

    public class Score
    {
        public long Id { get; set; }
        public long Rank { get; set; }
        public long BaseScore { get; set; }
        public long ModifiedScore { get; set; }
        public double Pp { get; set; }
        public long Weight { get; set; }
        public string Modifiers { get; set; }
        public long Multiplier { get; set; }
        public long BadCuts { get; set; }
        public long MissedNotes { get; set; }
        public long MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public long Hmd { get; set; }
        public DateTimeOffset TimeSet { get; set; }
        public bool HasReplay { get; set; }
    }
}