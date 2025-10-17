using System.Configuration;
using System.Data;
using System.Windows;
using POSApp.Data;

namespace POSApp
{
    public partial class App : Application
    {
        public User? CurrentUser { get; set; }
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                await MongoDb.TestConnectionAsync();
                await MongoDb.EnsureIndexesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database initialization failed: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }

            // Prevent the app from auto-closing when the login page closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var login = new LoginWindow();
            var ok = login.ShowDialog() == true;

            if (!ok)
            {
                Shutdown();
                return;
            }

            var main = new MainWindow();
            Current.MainWindow = main;
            main.ShowMenu();
            main.Show();

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}
