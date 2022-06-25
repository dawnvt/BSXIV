using BSXIV.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BSXIV
{
    public class DbContext
    {
        private LoggingUtils _logging;
        private static MongoClient _client;

        public DbContext(LoggingUtils logging)
        {
            _logging = logging;
            MongoConnection();
        }

        private void MongoConnection()
        {
            _client = new MongoClient(
                Environment.GetEnvironmentVariable("MONGODB")
                );
            var settings = 
                MongoClientSettings.FromConnectionString(
                    Environment.GetEnvironmentVariable("MONGODB")
                );
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(30);
                
            _logging.Log(LogSeverity.Info, "Started MongoDB connection successfully!");
        }

        public void Insert(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.InsertOne(document);
        }
        
        public void Find(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.Find(document).ToList();
        }
        
        public void FindOne(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.Find(document).First();
        }
        
        public void Update(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.UpdateOne(document, document);
        }
        
        public void UpdateMany(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.UpdateMany(document, document);
        }
        
        public void Delete(string collection, BsonDocument document)
        {
            var db = _client.GetDatabase("bsxiv");
            var dbCollection = db.GetCollection<BsonDocument>(collection);
            dbCollection.DeleteOne(document);
        }
    }
}