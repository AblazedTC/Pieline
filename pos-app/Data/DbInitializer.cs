using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace POSApp.Data
{
    public static class Db
    {
        private static readonly IConfiguration config;

        static Db()
        {
            try
            {
                config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load appsettings.json from {AppContext.BaseDirectory}\n\n{ex.Message}","Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public static string AppConn =>
            config.GetConnectionString("Default");

        public static async Task<MySqlConnection> OpenAsync()
        {
            var conn = new MySqlConnection(AppConn);
            await conn.OpenAsync();
            return conn;
        }

        public static async Task TestConnectionAsync()
        {
            await using var conn = new MySqlConnection(AppConn);
            await conn.OpenAsync();
        }
    }
}
