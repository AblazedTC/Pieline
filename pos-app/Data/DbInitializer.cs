using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSApp.Data
{
    public static class MongoDb
    {
        private static readonly Lazy<IMongoClient> _client = new(() =>
        {
            var connStr = Environment.GetEnvironmentVariable("MONGODB_URI");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("MONGODB_URI not set. Add it as an environment variable.");

            return new MongoClient(connStr);
        });

        public static IMongoClient Client => _client.Value;
        public static IMongoDatabase AppDb => Client.GetDatabase("pieline_db");
        public static IMongoCollection<User> Users => AppDb.GetCollection<User>("users");

        public static async Task TestConnectionAsync()
        {
            var cmd = new BsonDocument("ping", 1);
            await AppDb.RunCommandAsync<BsonDocument>(cmd);
        }

        public static async Task EnsureIndexesAsync()
        {
            var phoneKeys = Builders<User>.IndexKeys.Ascending(u => u.Phone);
            var phoneOpts = new CreateIndexOptions { Unique = true, Name = "uq_phone" };
            await Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(phoneKeys, phoneOpts));

            var emailKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
            var emailOpts = new CreateIndexOptions { Unique = true, Name = "uq_email" };
            await Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(emailKeys, emailOpts));
        }

    }
}
