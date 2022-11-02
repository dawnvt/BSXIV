using System.Runtime.CompilerServices;
using System.Text.Json;
using BSXIV.Utilities;
using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using NLog;
using SkiaSharp;
using ZstdSharp.Unsafe;

namespace BSXIV.BeatSaber.ScoreSaber
{
    [RequireContext(ContextType.Guild)]
    public class Main : InteractionModuleBase
    {
        private DbContext _dbContext;
        private WebRequest _webRequest;
        private Logger _logger;

        public Main(DbContext dbContext, WebRequest request, Logger logger)
        {
            _dbContext = dbContext;
            _webRequest = request;
            _logger = logger;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [SlashCommand("usercount", "ScoreSaber usercount")]
        private async Task UserCount()
        {
            await RespondAsync(_dbContext.Find("users", new BsonDocument()).Count.ToString());
        }

        [SlashCommand("adduser", "Adds your ScoreSaber user")]
        private async Task AddUser(string scoreSaberId)
        {
            var doc = new BsonDocument
            {
                { "scoreSaberId", scoreSaberId },
                { "userId", (long)Context.User.Id }
            };

            if (_dbContext.FindOne("users", new BsonDocument { { "userId", (long)Context.User.Id } }) == null)
            {
                _dbContext.Insert("users", doc);

                await RespondAsync($"User {scoreSaberId} added", ephemeral: true);
            }
            else
            {
                _dbContext.Update("users", new BsonDocument { { "userId", (long)Context.User.Id } }, doc);

                await RespondAsync($"User {scoreSaberId} updated", ephemeral: true);
            }
        }

        //[SlashCommand("deleteuser", "Deletes the user from the database")]
        //private async Task DeleteUser()
        //{
        //    if (Context.User.Id != null)
        //    {
        //        _dbContext.Delete("users", new BsonDocument { {  } })
        //    }
        //}

        [SlashCommand("topscore", "Shows the top score of your ScoreSaber user")]
        private async Task TopScore(string scoreSaberId = "")
        {
            if (scoreSaberId == string.Empty)
            {
                var user = _dbContext.FindOne("users", new BsonDocument { { "userId", (long)Context.User.Id } });
                if (user == null)
                {
                    await RespondAsync("You haven't added your ScoreSaber user yet. Use `/add <scoreSaberId>` to add your user.");
                    return;
                }

                scoreSaberId = user["scoreSaberId"].ToString();
            }

            var url = $"https://scoresaber.com/api/player/{scoreSaberId}/scores?limit=1&sort=top";
            var response = JsonSerializer.Deserialize<ScoreSaberScores>(await _webRequest.MakeRequestAsync(url).ConfigureAwait(false), Program.Options);

            url = $"https://scoresaber.com/api/player/{scoreSaberId}/basic";
            var userResponse = JsonSerializer.Deserialize<PlayerInfo>(await _webRequest.MakeRequestAsync(url).ConfigureAwait(false), Program.Options);

            if (response == null || userResponse == null)
            {
                await RespondAsync("Could not get score for user.");
                return;
            }

            var score = response.PlayerScores[0];

            var songName = score.Leaderboard.SongName;

            if (!string.IsNullOrWhiteSpace(score.Leaderboard.SongSubName))
                songName += " - " + score.Leaderboard.SongSubName;

            var embed = new EmbedBuilder()
                .WithTitle($"Top score for `{userResponse.Name}`")
                .WithThumbnailUrl(score.Leaderboard.CoverImage.AbsoluteUri)
                .WithDescription($"{songName}")
                .AddField("Rank", score.Score.Rank, true)
                .AddField("Score", score.Score.ModifiedScore, true)
                .AddField("Pp", score.Score.Pp, true)
                .AddField("Difficulty", score.Leaderboard.Difficulty.DifficultyString, true)
                .AddField("GameMode", score.Leaderboard.Difficulty.GameMode, true)
                .WithUrl($"https://scoresaber.com/leaderboard/{score.Leaderboard.Id}")
                .WithCurrentTimestamp();

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("recentscore", "Shows the most recent score of your ScoreSaber user")]
        public async Task RecentScore(string scoreSaberId = "")
        {
            if (scoreSaberId == string.Empty)
            {
                var user = _dbContext.FindOne("users", new BsonDocument { { "userId", (long)Context.User.Id } });
                if (user == null)
                {
                    await RespondAsync("You haven't added your ScoreSaber user yet. Use `/add <scoreSaberId>` to add your user.");
                    return;
                }

                scoreSaberId = user["scoreSaberId"].ToString();
            }

            var url = $"https://scoresaber.com/api/player/{scoreSaberId}/scores?limit=1&sort=recent";
            var response = JsonSerializer.Deserialize<ScoreSaberScores>(await _webRequest.MakeRequestAsync(url).ConfigureAwait(false), Program.Options);

            url = $"https://scoresaber.com/api/player/{scoreSaberId}/basic";
            var userResponse = JsonSerializer.Deserialize<PlayerInfo>(await _webRequest.MakeRequestAsync(url).ConfigureAwait(false), Program.Options);

            if (response == null || userResponse == null)
            {
                await RespondAsync("Could not get score for user.");
                return;
            }

            var score = response.PlayerScores[0];

            var songName = score.Leaderboard.SongName;

            if (!string.IsNullOrWhiteSpace(score.Leaderboard.SongSubName))
                songName += " - " + score.Leaderboard.SongSubName;

            var embed = new EmbedBuilder()
                .WithTitle($"Recent score for `{userResponse.Name}`")
                .WithThumbnailUrl(score.Leaderboard.CoverImage.AbsoluteUri)
                .WithDescription($"{songName}")
                .AddField("Rank", score.Score.Rank, true)
                .AddField("Score", score.Score.ModifiedScore, true)
                .AddField("Pp", score.Score.Pp, true)
                .AddField("Difficulty", score.Leaderboard.Difficulty.DifficultyString, true)
                .AddField("GameMode", score.Leaderboard.Difficulty.GameMode, true)
                .WithUrl($"https://scoresaber.com/leaderboard/{score.Leaderboard.Id}")
                .WithCurrentTimestamp();

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("newestqualified", "Shows the most recent qualified ScoreSaber map")]
        private async Task NewestQualified()
        {
            var url = $"https://scoresaber.com/api/leaderboards?qualified=true&sort=0";

            var result = JsonSerializer.Deserialize<NewestLeaderboard>(await _webRequest.MakeRequestAsync(url).ConfigureAwait(false)!, Program.Options);

            var image = await _webRequest.MakeRequestAsync(result?.Leaderboards[0].CoverImage).ConfigureAwait(false);

            var bitmap = SKBitmap.Decode(image);

            int total = 0, r = 0, g = 0, b = 0;

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var clr = bitmap.GetPixel(x, y);
                    r += clr.Red;
                    g += clr.Green;
                    b += clr.Blue;

                    total++;
                }
            }

            r /= total;
            g /= total;
            b /= total;

            var embed = new EmbedBuilder()
                .WithFooter(footer => footer.Text = "Newest Qualified")
                .WithColor(r, g, b)
                .WithTitle($"{result?.Leaderboards[0].SongName} - {result?.Leaderboards[0].SongSubName}")
                .WithImageUrl(result?.Leaderboards[0].CoverImage.AbsoluteUri)
                .WithCurrentTimestamp();

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("newestranked", "Shows the most recent ranked ScoreSaber map")]
        private async Task NewestRanked()
        {
            var url = $"https://scoresaber.com/api/leaderboards?ranked=true&sort=0";

            var result = JsonSerializer.Deserialize<NewestLeaderboard>(_webRequest.MakeRequestAsync(url).ConfigureAwait(false).GetAwaiter().GetResult(), Program.Options);

            var image = await _webRequest.MakeRequestAsync(result?.Leaderboards[0].CoverImage).ConfigureAwait(false);

            var bitmap = SKBitmap.Decode(image);

            int total = 0, r = 0, g = 0, b = 0;

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var clr = bitmap.GetPixel(x, y);
                    r += clr.Red;
                    g += clr.Green;
                    b += clr.Blue;

                    total++;
                }
            }

            r /= total;
            g /= total;
            b /= total;

            var songName = result?.Leaderboards[0].SongName;

            if (!string.IsNullOrWhiteSpace(result?.Leaderboards[0].SongSubName))
                songName += " - " + result?.Leaderboards[0].SongSubName;

            var embed = new EmbedBuilder()
                .WithFooter(footer => footer.Text = "Newest Qualified")
                .WithColor(r, g, b)
                .WithTitle(songName)
                .WithImageUrl(result?.Leaderboards[0].CoverImage.AbsoluteUri)
                .WithCurrentTimestamp();

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("leaderboard", "Shows the leaderboard for a given ScoreSaber map")]
        private async Task ShowLeaderboard(int leaderboardId)
        {
            var url = $"https://scoresaber.com/api/leaderboard/by-id/{leaderboardId}/scores";
            var scoreResponse = await _webRequest.MakeRequestAsync(url).ConfigureAwait(false);
            url = $"https://scoresaber.com/api/leaderboard/by-id/{leaderboardId}/info";
            var infoResponse = await _webRequest.MakeRequestAsync(url).ConfigureAwait(false);

            if (scoreResponse == null || infoResponse == null)
            {
                await RespondAsync($"No leaderboard found for `{leaderboardId}`. Did you specify the right leaderboard id?");
            }
            else
            {
                var leaderboardScores = JsonSerializer.Deserialize<LeaderboardScores>(scoreResponse, Program.Options);
                var leaderboard = JsonSerializer.Deserialize<Leaderboard>(infoResponse, Program.Options);

                if (leaderboardScores == null)
                {
                    await RespondAsync($"Response gotten from {leaderboardId} was empty. Something was wrong.");
                }

                var songName = leaderboard?.SongName;

                if (!string.IsNullOrWhiteSpace(leaderboard?.SongSubName))
                    songName += " - " + leaderboard?.SongSubName;

                var embed = new EmbedBuilder()
                    .WithTitle($"{songName}")
                    .AddField("Song Author", leaderboard?.SongAuthorName, true)
                    .AddField("Difficulty", leaderboard?.Difficulty.DifficultyString, true)
                    .AddField("GameMode", leaderboard?.Difficulty.GameMode, true)
                    .WithColor(0xffde1a)
                    .WithThumbnailUrl(leaderboard.CoverImage.AbsoluteUri)
                    .WithUrl($"https://scoresaber.com/leaderboard/{leaderboardId}")
                    .WithCurrentTimestamp();

                if (leaderboardScores?.Scores != null)
                {
                    foreach (var score in leaderboardScores.Scores.Take(10))
                    {
                        embed.AddField($"{score.Rank} - {score.LeaderboardPlayerInfo.Name}",
                            $"{score.ModifiedScore} - {score.Pp}pp");
                    }
                }

                await RespondAsync(embed: embed.Build());
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

    public class NewestLeaderboard
    {
        public Leaderboard[] Leaderboards { get; set; }
        public Metadata Metadata { get; set; }
    }

    public class LeaderboardScores
    {
        public Score[] Scores { get; set; }
        public Metadata Metadata { get; set; }
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
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? RankedDate { get; set; }
        public DateTimeOffset? QualifiedDate { get; set; }
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
        public Difficulty[] Difficulties { get; set; }
    }

    public class Difficulty
    {
        public long LeaderboardId { get; set; }
        public long DifficultyDifficulty { get; set; }
        public string GameMode { get; set; }
        public string DifficultyRaw { get; set; }

        public string DifficultyString
        {
            get
            {
                var segments = DifficultyRaw.Split('_', StringSplitOptions.RemoveEmptyEntries);
                return $"{segments[0]}";
            }
        }


    }

    public class Score
    {
        public long Id { get; set; }
        public PlayerInfo LeaderboardPlayerInfo { get; set; }
        public long Rank { get; set; }
        public long BaseScore { get; set; }
        public long ModifiedScore { get; set; }
        public double Pp { get; set; }
        public double Weight { get; set; }
        public string Modifiers { get; set; }
        public float Multiplier { get; set; }
        public long BadCuts { get; set; }
        public long MissedNotes { get; set; }
        public long MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public long Hmd { get; set; }
        public DateTimeOffset? TimeSet { get; set; }
        public bool HasReplay { get; set; }
    }

    public partial class PlayerInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri ProfilePicture { get; set; }
        public string Country { get; set; }
        public long Permissions { get; set; }
        public string? Badges { get; set; }
        public string? Role { get; set; }
    }
}