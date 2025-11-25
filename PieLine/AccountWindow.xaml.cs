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
            PasswordBox.Password = string.Empty;

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
            // clear prior error and success
            CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, null);
            if (this.FindName("AccountSuccessBorder") is Border sb) sb.Visibility = Visibility.Collapsed;

            // ensure a user is signed in
            string originalPhone = CurrentUser.Phone ?? string.Empty;
            string originalEmail = CurrentUser.Email ?? string.Empty;
            if (string.IsNullOrWhiteSpace(originalPhone) && string.IsNullOrWhiteSpace(originalEmail))
            {
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Error: No user is currently signed in.");
                return;
            }

            string newFullName = FullNameTextBox.Text?.Trim() ?? string.Empty;
            string newPhoneRaw = PhoneNumberTextBox.Text?.Trim() ?? string.Empty;
            string newEmail = EmailTextBox.Text?.Trim() ?? string.Empty;
            string newPassword = PasswordBox.Password?.Trim() ?? string.Empty;

            // basic validation similar to register
            if (string.IsNullOrWhiteSpace(newFullName))
            {
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Error: Invalid name, please enter a valid name, then try again.");
                return;
            }

            string phoneDigits = CommonHelpers.ExtractDigits(newPhoneRaw);
            if (string.IsNullOrWhiteSpace(phoneDigits) || phoneDigits.Length !=10)
            {
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Error: Invalid phone number, please enter a valid phone number then try again.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@") || !newEmail.Contains('.'))
            {
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Error: Invalid email address, please enter a valid email address then try again.");
                return;
            }

            // Only apply password change if provided and different from current stored password
            var users = UserFile.LoadUsers();
            var currentUser = users.FirstOrDefault(u => u.PhoneNumber == originalPhone || u.Email.Equals(originalEmail, StringComparison.OrdinalIgnoreCase));
            bool passwordChanged = false;
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (currentUser != null && currentUser.Password == newPassword)
                {
                    // password same as existing, ignore
                    passwordChanged = false;
                }
                else
                {
                    if (!UserFile.IsValidPassword(newPassword))
                    {
                        CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Error: Passwords must contain a number0-9, capital letter A-Z, and be more then8 characters.");
                        return;
                    }

                    // apply change
                    if (!UserFile.TryResetPassword(originalPhone, originalEmail, newPassword, out string pwdError))
                    {
                        CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, pwdError);
                        return;
                    }
                    passwordChanged = true;
                }
            }

            if (UserFile.TryUpdateUser(originalPhone, newFullName, newEmail, phoneDigits, out string errorMessage))
            {
                // update in-memory current user
                CurrentUser.Name = newFullName;
                CurrentUser.Phone = phoneDigits;
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
                                    o.Phone = phoneDigits;
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
                    CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, "Account updated but failed to update existing orders.");
                }

                // show success green box
                var successBorder = this.FindName("AccountSuccessBorder") as Border;
                if (successBorder != null)
                {
                    successBorder.Visibility = Visibility.Visible;
                }
                // hide any error
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, null);

                // clear password box after successful change
                PasswordBox.Password = string.Empty;

                // reload last order (in case phone/email changed)
                LoadOrdersList();
            }
            else
            {
                CommonHelpers.SetError(AccountErrorBorder, AccountErrorTextBlock, errorMessage);
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

        private void PhoneNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // allow only digits and limit to10 characters
            if (sender is TextBox tb)
            {
                // if input is not digit, reject
                if (!e.Text.All(char.IsDigit))
                {
                    e.Handled = true;
                    return;
                }

                // if resulting length would exceed10, reject
                int selectionLength = tb.SelectionLength;
                int currentLength = tb.Text.Length;
                int newLength = currentLength - selectionLength + e.Text.Length;
                if (newLength >10)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void PhoneNumberTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e.DataObject.GetDataPresent(DataFormats.Text))
                {
                    string pasted = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
                    string digits = new string(pasted.Where(char.IsDigit).ToArray());
                    if (string.IsNullOrEmpty(digits))
                    {
                        e.CancelCommand();
                        return;
                    }

                    int selectionLength = tb.SelectionLength;
                    int currentLength = tb.Text.Length;
                    int newLength = currentLength - selectionLength + digits.Length;
                    if (newLength >10)
                    {
                        // trim to fit
                        int allowed =10 - (currentLength - selectionLength);
                        if (allowed <=0)
                        {
                            e.CancelCommand();
                            return;
                        }

                        string trimmed = digits.Substring(0, allowed);
                        var data = new DataObject();
                        data.SetData(DataFormats.Text, trimmed);
                        e.DataObject = data;
                    }
                }
                else
                {
                    e.CancelCommand();
                }
            }
        }

    }
}
