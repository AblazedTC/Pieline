using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;

namespace PieLine
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private OrderRecord? lastOrder;

        public AccountWindow()
        {
            InitializeComponent();

            // populate account fields with current user info
            FullNameTextBox.Text = string.IsNullOrWhiteSpace(CurrentUser.Name) ? string.Empty : CurrentUser.Name;
            PhoneNumberTextBox.Text = string.IsNullOrWhiteSpace(CurrentUser.Phone) ? string.Empty : CurrentUser.Phone;
            EmailTextBox.Text = string.IsNullOrWhiteSpace(CurrentUser.Email) ? string.Empty : CurrentUser.Email;

            LoadOrdersList();
        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var companyInfoWindow = new CompanyInformationWindow();
            companyInfoWindow.Show();
            this.Close();
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
            this.Close();
        }
        private void UpdateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // ensure a user is signed in
            string originalPhone = CurrentUser.Phone ?? string.Empty;
            string originalEmail = CurrentUser.Email ?? string.Empty;
            if (string.IsNullOrWhiteSpace(originalPhone) && string.IsNullOrWhiteSpace(originalEmail))
            {
                MessageBox.Show("No user is currently signed in.", "Update Account", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string newFullName = FullNameTextBox.Text?.Trim() ?? string.Empty;
            string newPhone = PhoneNumberTextBox.Text?.Trim() ?? string.Empty;
            string newEmail = EmailTextBox.Text?.Trim() ?? string.Empty;

            if (UserFile.TryUpdateUser(originalPhone, newFullName, newEmail, newPhone, out string errorMessage))
            {
                // update in-memory current user
                CurrentUser.Name = newFullName;
                CurrentUser.Phone = newPhone;
                CurrentUser.Email = newEmail;

                // update any existing orders that belong to this user (matching old phone or old email)
                try
                {
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;
                    string ordersPath = System.IO.Path.Combine(appDir, "orders.json");

                    if (File.Exists(ordersPath))
                    {
                        string ordersJson = File.ReadAllText(ordersPath);
                        if (!string.IsNullOrWhiteSpace(ordersJson))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var orders = JsonSerializer.Deserialize<List<OrderRecord>>(ordersJson, options) ?? new List<OrderRecord>();

                            bool changed = false;
                            foreach (var o in orders)
                            {
                                if ((!string.IsNullOrWhiteSpace(originalPhone) && o.Phone == originalPhone) ||
                                    (!string.IsNullOrWhiteSpace(originalEmail) && o.Email.Equals(originalEmail, StringComparison.OrdinalIgnoreCase)))
                                {
                                    o.CustomerName = newFullName;
                                    o.Phone = newPhone;
                                    o.Email = newEmail;
                                    changed = true;
                                }
                            }

                            if (changed)
                            {
                                var writeOptions = new JsonSerializerOptions { WriteIndented = true };
                                string output = JsonSerializer.Serialize(orders, writeOptions);
                                File.WriteAllText(ordersPath, output);
                            }
                        }
                    }
                }
                catch
                {
                    // ignore errors updating orders.json but notify user
                    MessageBox.Show("Account updated but failed to update existing orders.", "Update Account", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                MessageBox.Show("Account updated successfully.", "Update Account", MessageBoxButton.OK, MessageBoxImage.Information);
                // reload last order (in case phone/email changed)
                LoadOrdersList();
            }
            else
            {
                MessageBox.Show(errorMessage, "Update Account", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ViewReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListBox.SelectedItem is OrderRecord selected)
            {
                var orderWindow = new ViewOrderRecieptWindow(selected.OrderId, selected);
                orderWindow.Show();
                this.Close();
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LoadOrdersList()
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = System.IO.Path.Combine(appDir, "orders.json");

                if (!File.Exists(path))
                {
                    OrdersListBox.ItemsSource = null;
                    NoOrdersTextBlock.Visibility = Visibility.Visible;
                    ViewReceiptButton.IsEnabled = false;
                    return;
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    OrdersListBox.ItemsSource = null;
                    NoOrdersTextBlock.Visibility = Visibility.Visible;
                    ViewReceiptButton.IsEnabled = false;
                    return;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orders = JsonSerializer.Deserialize<List<OrderRecord>>(json, options) ?? new List<OrderRecord>();

                // filter orders to the current signed-in user
                string curPhone = CurrentUser.Phone ?? string.Empty;
                string curEmail = CurrentUser.Email ?? string.Empty;

                var userOrders = orders.Where(o =>
                 (!string.IsNullOrWhiteSpace(curPhone) && o.Phone == curPhone) ||
                 (!string.IsNullOrWhiteSpace(curEmail) && o.Email.Equals(curEmail, StringComparison.OrdinalIgnoreCase))
                ).OrderByDescending(o => o.OrderDate).ToList();

                if (userOrders.Count ==0)
                {
                    OrdersListBox.ItemsSource = null;
                    NoOrdersTextBlock.Visibility = Visibility.Visible;
                    ViewReceiptButton.IsEnabled = false;
                    return;
                }

                OrdersListBox.ItemsSource = userOrders;
                NoOrdersTextBlock.Visibility = Visibility.Collapsed;
                ViewReceiptButton.IsEnabled = false; // disabled until selection
            }
            catch
            {
                OrdersListBox.ItemsSource = null;
                NoOrdersTextBlock.Visibility = Visibility.Visible;
                ViewReceiptButton.IsEnabled = false;
            }
        }

        private void OrdersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewReceiptButton.IsEnabled = OrdersListBox.SelectedItem is OrderRecord;
        }

        private void ShowNoOrders()
        {
            OrdersListBox.ItemsSource = null;
            NoOrdersTextBlock.Visibility = Visibility.Visible;
            ViewReceiptButton.IsEnabled = false;
        }

    }
}
