using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace POSApp.Data
{
    public static class Db
    {
        public static string AppConn(string dbName) =>
            $"Server=127.0.0.1;Port=3306;Database={dbName};User ID=appuser;Password=admin123;SslMode=None;Connection Timeout=3";
        public static async Task<MySqlConnection> OpenAsync(string dbName)
        {
            var conn = new MySqlConnection(AppConn(dbName));
            await conn.OpenAsync();
            return conn;
        }

        public static async Task TestConnectionAsync(string dbName)
        {
            await using var conn = new MySqlConnection(AppConn(dbName));
            await conn.OpenAsync();
        }
    }
}
