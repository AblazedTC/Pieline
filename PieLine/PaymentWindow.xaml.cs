using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PieLine
{
    public partial class PaymentWindow : Window
    {
        private enum DeliveryOption {None, Pickup, Delivery}
        private enum PaymentOption {None, Cash, Card}

        private readonly List<MenuItem> items;

        private DeliveryOption delivery = DeliveryOption.None;
        private PaymentOption payment = PaymentOption.None;

        public PaymentWindow(List<MenuItem> items)
        {
            InitializeComponent();
            this.items = items ?? new List<MenuItem>();
        }

        private void SetError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Text = string.Empty;
            }
            else
            {
                ErrorTextBlock.Text = message;
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void SetSelected(Button button, bool selected)
        {
            if (selected)
            {
                button.Background = (Brush)FindResource("AccentRed");
                button.Foreground = Brushes.White;
            }
            else
            {
                button.Background = Brushes.White;
                button.Foreground = (Brush)FindResource("AccentRed");
            }
        }

        private void UpdateDeliveryButtons()
        {
            SetSelected(PickupButton, delivery == DeliveryOption.Pickup);
            SetSelected(DeliveryButton, delivery == DeliveryOption.Delivery);

            DeliveryAddressPanel.Visibility =
                delivery == DeliveryOption.Delivery ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePaymentButtons()
        {
            SetSelected(CashButton, payment == PaymentOption.Cash);
            SetSelected(CardButton, payment == PaymentOption.Card);

            CardInfoPanel.Visibility =
                payment == PaymentOption.Card ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PickupButton_Click(object sender, RoutedEventArgs e)
        {
            delivery = DeliveryOption.Pickup;
            UpdateDeliveryButtons();
            SetError(null);
        }

        private void DeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            delivery = DeliveryOption.Delivery;
            UpdateDeliveryButtons();
            SetError(null);
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            payment = PaymentOption.Cash;
            UpdatePaymentButtons();
            SetError(null);
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            payment = PaymentOption.Card;
            UpdatePaymentButtons();
            SetError(null);
        }

        private bool ValidateSelections()
        {
            if (delivery == DeliveryOption.None || payment == PaymentOption.None)
            {
                SetError("Error: Invalid payment/delivery option, please select a delivery option, and a payment option to continue.");
                return false;
            }

            return true;
        }

        private bool ValidateDeliveryAddress()
        {
            if (delivery != DeliveryOption.Delivery)
                return true;

            bool hasStreet = !string.IsNullOrWhiteSpace(StreetTextBox.Text);
            bool hasCity = !string.IsNullOrWhiteSpace(CityTextBox.Text);
            bool hasState = !string.IsNullOrWhiteSpace(StateTextBox.Text);
            bool hasZip = !string.IsNullOrWhiteSpace(ZipTextBox.Text);

            if (hasStreet && hasCity && hasState && hasZip)
                return true;

            SetError("Error: Invalid delivery address, please enter a valid delivery address to continue.");
            return false;
        }

        private bool ValidateCardInfo()
        {
            if (payment != PaymentOption.Card)
                return true;

            string name = CardholderNameTextBox.Text?.Trim() ?? "";
            string numberDigits = Regex.Replace(CardNumberTextBox.Text ?? "", @"\D", "");
            string expiry = ExpiryDateTextBox.Text?.Trim() ?? "";
            string cvcDigits = Regex.Replace(SecurityCodeTextBox.Text ?? "", @"\D", "");

            bool nameOk = name.Length >= 3;
            bool numOk = numberDigits.Length >= 12 && numberDigits.Length <= 19;
            bool expiryOk = expiry.Length is 4 or 5;
            bool cvcOk = cvcDigits.Length == 3;

            if (nameOk && numOk && expiryOk && cvcOk)
                return true;

            SetError("Error: Invalid card information, please enter valid card information to continue.");
            return false;
        }

        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            SetError(null);

            if (!ValidateSelections())
                return;

            if (!ValidateDeliveryAddress())
                return;

            if (!ValidateCardInfo())
                return;

            bool isDeliverySelected = (delivery == DeliveryOption.Delivery);

            string addressLabel;
            string addressText;

            if (isDeliverySelected)
            {
                addressLabel = "Delivery Address";
                addressText =
                    $"{StreetTextBox.Text}\n{CityTextBox.Text}, {StateTextBox.Text}, {ZipTextBox.Text}";
            }
            else
            {
                addressLabel = "Pickup Location";
                addressText = "680 Artson Rd, Suite 161\nMarietta, GA, 30060";
            }

            string paymentSummary;
            if (payment == PaymentOption.Card)
            {
                var digits = new string(CardNumberTextBox.Text.Where(char.IsDigit).ToArray());
                var last4 = digits.Length >= 4 ? digits[^4..] : digits;
                paymentSummary = $"Credit / Debit Card - Ending in {last4}";
            }
            else
            {
                paymentSummary = "Cash";
            }

            var reviewWindow = new ReviewWindow(
                items,
                addressLabel,
                addressText,
                paymentSummary,
                isDeliverySelected);

            reviewWindow.Show();
            Close();
        }

        private void CardNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            bool isAllowedChar = char.IsDigit(c) || c == ' ' || c == '-';

            if (!isAllowedChar)
            {
                e.Handled = true;
                return;
            }

            if (sender is TextBox tb)
            {
                int currentDigitCount = tb.Text.Count(ch => char.IsDigit(ch));
                int newDigitCount = currentDigitCount + (char.IsDigit(c) ? 1 : 0);

                if (newDigitCount > 16)
                {
                    e.Handled = true;
                }
            }
        }

        private void ExpiryTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();

            if (sender is not TextBox tb)
            {
                e.Handled = true;
                return;
            }

            if (char.IsDigit(c))
                return;

            if (c == '/')
            {
                if (tb.Text.Contains('/'))
                    e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void NumbersTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            if (!char.IsDigit(c))
                e.Handled = true;
        }

        private void LettersSpaceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            if (!char.IsLetter(c) && c != ' ')
                e.Handled = true;
        }

        private void StreetTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            if (!char.IsLetterOrDigit(c) && c != ' ')
                e.Handled = true;
        }

        private void BlockSpaceCharacter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
