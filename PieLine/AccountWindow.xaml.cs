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

            LoadLastOrder();
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
            return;
            
        }
        private void ViewReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastOrder == null)
                return;

            // Open a simple order details window or reuse OrderPlacedWindow
            var orderWindow = new ViewOrderRecieptWindow(lastOrder.OrderId, lastOrder);
            orderWindow.Show();
            this.Close();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LoadLastOrder()
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = System.IO.Path.Combine(appDir, "orders.json");

                if (!File.Exists(path))
                {
                    ShowNoOrders();
                    return;
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ShowNoOrders();
                    return;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orders = JsonSerializer.Deserialize<List<OrderRecord>>(json, options);

                if (orders == null || orders.Count ==0)
                {
                    ShowNoOrders();
                    return;
                }

                // pick the last order by OrderDate
                lastOrder = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();

                if (lastOrder == null)
                {
                    ShowNoOrders();
                    return;
                }

                // populate UI
                OrderNumberTextBlock.Text = $"Order Number: {lastOrder.OrderId}";
                ItemAmountTextBlock.Text = $"Item Amount: {lastOrder.Items?.Count ??0}";
                OrderedTextBlock.Text = $"Ordered: {lastOrder.OrderDate.ToString("MM/dd/yyyy HH:mm")}";
                PaymentTextBlock.Text = $"Payment: {lastOrder.PaymentSummary}";
                TotalTextBlock.Text = $"Total: {lastOrder.Total.ToString("C2")}";

                LastOrderBorder.Visibility = Visibility.Visible;
                NoOrdersTextBlock.Visibility = Visibility.Collapsed;
                ViewReceiptButton.IsEnabled = true;
            }
            catch
            {
                ShowNoOrders();
            }
        }

        private void ShowNoOrders()
        {
            LastOrderBorder.Visibility = Visibility.Collapsed;
            NoOrdersTextBlock.Visibility = Visibility.Visible;
            ViewReceiptButton.IsEnabled = false;
        }

    }
}
