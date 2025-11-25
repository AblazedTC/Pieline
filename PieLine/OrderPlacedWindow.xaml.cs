using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace PieLine
{
    public partial class OrderPlacedWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        public OrderPlacedWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ViewReceipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = System.IO.Path.Combine(appDir, "orders.json");

                if (!File.Exists(path))
                {
                    MessageBox.Show("No receipt found.", "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("No receipt found.", "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orders = JsonSerializer.Deserialize<List<OrderRecord>>(json, options);

                if (orders == null || orders.Count == 0)
                {
                    MessageBox.Show("No receipt found.", "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var last = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();
                if (last == null)
                {
                    MessageBox.Show("No receipt found.", "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var win = new ViewOrderRecieptWindow(last.OrderId, last);
                win.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open receipt: {ex.Message}", "Receipt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            var info = new CompanyInformationWindow();
            info.Show();
            this.Close();
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var info = new CompanyInformationWindow();
            info.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new AccountWindow();
            accountWindow.Show();
            this.Close();
        }
    }
}
