using BSXIV.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BSXIV
{
    public class DbContext
    {
        private static MongoClient _client;
        private static ILogger _logger;

        public DbContext(ILogger<DbContext> logger)
        {
            _logger = logger;
            MongoConnection();
        }

        private void MongoConnection()
        {
            var variables = Environment.GetEnvironmentVariables();

            var conString = Environment.GetEnvironmentVariable("MONGODB");

            var settings = MongoClientSettings.FromConnectionString(conString);
#if DEBUG
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
#else
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
#endif
            _client = new MongoClient(settings);
            
            _logger.LogInformation("Successfully connected to the MongoDB Instance!");
            _client.StartSession();
            
        }

        public void Insert(string collection, BsonDocument insert)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.InsertOne(insert);
        }

        public List<BsonDocument> Find(string collection, BsonDocument search)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            return dbCollection.Find(search).ToList();
        }

        public BsonDocument? FindOne(string collection, BsonDocument search)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            return dbCollection.Find(search).FirstOrDefault();
        }

        public void Update(string collection, BsonDocument search, BsonDocument set)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.UpdateOne(search, set);
        }

        public void UpdateMany(string collection, BsonDocument search, BsonDocument set)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.UpdateMany(search, set);
        }

        public void Delete(string collection, BsonDocument search)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.DeleteOne(search);
        }
    }
}