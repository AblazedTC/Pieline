using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PieLine
{
    public partial class ReviewWindow : Window
    {
        private readonly List<MenuItem> items;
        private readonly bool isDelivery;

        private const decimal TaxRate = 0.08m;
        private const decimal DeliveryFeeAmount = 3.99m;

        public ReviewWindow(
            List<MenuItem> items,
            string addressTitle,
            string addressBody,
            string paymentSummary,
            bool isDelivery)
        {
            InitializeComponent();

            this.items = items ?? new List<MenuItem>();
            this.isDelivery = isDelivery;

            AddressTitleTextBlock.Text = addressTitle;
            AddressBodyTextBlock.Text = addressBody;

            PaymentSummaryTextBlock.Text = paymentSummary;

            ItemsList.ItemsSource = this.items;

            CalculateTotals();
        }

        private void CalculateTotals()
        {
            decimal subtotal = items.Sum(i => i.Price);
            decimal tax = Math.Round(subtotal * TaxRate, 2);
            decimal deliveryFee = isDelivery ? DeliveryFeeAmount : 0m;
            decimal total = subtotal + tax + deliveryFee;

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

        private void EditPayment_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var paymentWindow = new PaymentWindow(items);
            paymentWindow.Show();
            Close();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Order placed! (stubbed)", "Order Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
