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

namespace PieLine
{
    /// <summary>
    /// Interaction logic for ViewOrderRecieptWindow.xaml
    /// </summary>
    public partial class ViewOrderRecieptWindow : Window
    {
        public ViewOrderRecieptWindow(String order_id, OrderRecord order)
        {
            InitializeComponent();
            Populate(order);
        }

        private void Populate(OrderRecord? order)
        {
            if (order == null)
            {
                MessageBox.Show("Order not found.", "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            OrderNumberText.Text = order.OrderId ?? string.Empty;
            OrderDateText.Text = order.OrderDate.ToString("MM/dd/yyyy h:mm tt");

            // Store address remains static in XAML but keep value if needed
            StoreAddressText.Text = string.IsNullOrWhiteSpace(StoreAddressText.Text) ? "680 Arntson Rd, Suite161, Marietta, GA,30060" : StoreAddressText.Text;

            CustomerNameText.Text = order.CustomerName ?? string.Empty;
            CustomerPhoneText.Text = order.Phone ?? string.Empty;
            CustomerEmailText.Text = order.Email ?? string.Empty;

            // Delivery details
            if (order.IsDelivery)
            {
                DeliveryDetailsPanel.Visibility = Visibility.Visible;
                DeliveryAddressText.Text = (order.AddressTitle + "\n" + order.AddressBody).Trim();
                DeliveryFeeRow.Visibility = Visibility.Visible;
            }
            else
            {
                DeliveryDetailsPanel.Visibility = Visibility.Collapsed;
                DeliveryFeeRow.Visibility = Visibility.Collapsed;
            }

            // Items: map OrderItemRecord to a display object expected by the DataTemplate
            var items = (order.Items ?? new List<OrderItemRecord>())
                .Select(i => new
                {
                    Name = i.Name ?? string.Empty,
                    Price = i.Price,
                    Details = string.Empty,
                    UnitPrice = i.Price,
                    Quantity = 1,
                    Total = i.Price
                })
                .ToList();

            ItemsList.ItemsSource = items;

            PaymentMethodText.Text = order.PaymentSummary ?? string.Empty;

            SubtotalText.Text = order.Subtotal.ToString("C2");
            TaxText.Text = order.Tax.ToString("C2");
            DeliveryFeeText.Text = order.DeliveryFee.ToString("C2");
            TotalPaidText.Text = order.Total.ToString("C2");
        }

        private void BackToAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var account = new AccountWindow();
            account.Show();
            this.Close();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var companyInfoWindow = new CompanyInformationWindow();
            companyInfoWindow.Show();
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
            var account = new AccountWindow();
            account.Show();
            this.Close();
        }
    }
}
