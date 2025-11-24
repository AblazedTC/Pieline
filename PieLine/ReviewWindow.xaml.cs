using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;

namespace PieLine
{
    public partial class ReviewWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        private readonly List<MenuItem> items;
        private readonly bool isDelivery;

        private const decimal TaxRate = 0.08m;
        private const decimal DeliveryFeeAmount = 3.99m;

        private decimal subtotal;
        private decimal tax;
        private decimal deliveryFee;
        private decimal total;

        public ReviewWindow(
            List<MenuItem> items,
            string addressTitle,
            string addressBody,
            string paymentSummary,
            bool isDelivery)
        {
            InitializeComponent();
            DataContext = this;

            this.items = items ?? new List<MenuItem>();
            this.isDelivery = isDelivery;

            AddressTitleTextBlock.Text = addressTitle;
            AddressBodyTextBlock.Text = addressBody;
            PaymentSummaryTextBlock.Text = paymentSummary;

            ItemsList.ItemsSource = this.items;

            CalculateTotals();
        }

        private void EditAddress_Click(object sender, RoutedEventArgs e)
        {
            var payment = new PaymentWindow(items);
            payment.Show();
            Close();
        }

        private void EditPayment_Click(object sender, RoutedEventArgs e)
        {
            var payment = new PaymentWindow(items);
            payment.Show();
            Close();
        }

        private void BackToPayment_Click(object sender, RoutedEventArgs e)
        {
            var payment = new PaymentWindow(items);
            payment.Show();
            Close();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var info = new CompanyInformationWindow();
            info.Show();
            Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            Close();
        }

        private void CalculateTotals()
        {
            subtotal = items.Sum(i => i.Price);
            tax = Math.Round(subtotal * TaxRate, 2);
            deliveryFee = isDelivery ? DeliveryFeeAmount : 0m;
            total = subtotal + tax + deliveryFee;

            SubtotalTextBlock.Text = subtotal.ToString("C2");
            TaxTextBlock.Text = tax.ToString("C2");
            DeliveryFeeTextBlock.Text = deliveryFee.ToString("C2");
            TotalTextBlock.Text = total.ToString("C2");
        }

        private void PaymentStepButton_Click(object sender, RoutedEventArgs e)
        {
            var paymentWindow = new PaymentWindow(items);
            paymentWindow.Show();
            Close();
        }

        private void EditPayment_Click(object sender, MouseButtonEventArgs e)
        {
            var paymentWindow = new PaymentWindow(items);
            paymentWindow.Show();
            Close();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            SaveOrderToFile();

            var win = new OrderPlacedWindow();
            win.Show();
            Close();
        }

        private void SaveOrderToFile()
        {
            decimal subtotal = items.Sum(i => i.Price);
            decimal tax = Math.Round(subtotal * TaxRate, 2);
            decimal deliveryFee = isDelivery ? DeliveryFeeAmount : 0m;
            decimal total = subtotal + tax + deliveryFee;

            string customerName = string.IsNullOrWhiteSpace(CurrentUser.Name)
                ? "Unknown"
                : CurrentUser.Name;

            string phone = string.IsNullOrWhiteSpace(CurrentUser.Phone)
                ? "Unknown"
                : CurrentUser.Phone;

            string email = string.IsNullOrWhiteSpace(CurrentUser.Email)
                ? "Unknown"
                : CurrentUser.Email;

            var order = new OrderRecord
            {
                OrderId = $"ORD-{Random.Shared.Next(10000000, 99999999)}",
                OrderDate = DateTime.Now,

                CustomerName = customerName,
                Phone = phone,
                Email = email,

                IsDelivery = this.isDelivery,
                AddressTitle = AddressTitleTextBlock.Text,
                AddressBody = AddressBodyTextBlock.Text,

                PaymentSummary = PaymentSummaryTextBlock.Text,

                Subtotal = subtotal,
                Tax = tax,
                DeliveryFee = deliveryFee,
                Total = total,

                Items = items.Select(i => new OrderItemRecord
                {
                    Name = i.Name,
                    Price = i.Price
                }).ToList()
            };

            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(appDir, "orders.json");

            List<OrderRecord> existingOrders;

            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    existingOrders = string.IsNullOrWhiteSpace(json)
                        ? new List<OrderRecord>()
                        : JsonSerializer.Deserialize<List<OrderRecord>>(json) ?? new List<OrderRecord>();
                }
                catch
                {
                    existingOrders = new List<OrderRecord>();
                }
            }
            else
            {
                existingOrders = new List<OrderRecord>();
            }

            existingOrders.Add(order);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string outputJson = JsonSerializer.Serialize(existingOrders, options);
            File.WriteAllText(path, outputJson);
        }
    }

    public class OrderItemRecord
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderRecord
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public bool IsDelivery { get; set; }
        public string AddressTitle { get; set; }
        public string AddressBody { get; set; }

        public string PaymentSummary { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Total { get; set; }

        public List<OrderItemRecord> Items { get; set; }
    }
}
