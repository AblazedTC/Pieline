using MongoDB.Driver;
using POSApp.Data;
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

namespace POSApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void onLoginClick(object sender, RoutedEventArgs e)
        {
            var rawPhone = UsernameBox.Text ?? "";
            var phone = NormalizePhone(rawPhone);
            var password = PasswordBox.Password ?? "";

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username (phone number) and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = await TryLoginAsync(phone, password);

                if (!result.found)
                {
                    MessageBox.Show("Phone number not found.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var u = result.user!;
                if (u.UserType.Equals("customer", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"{u.FullName} is not an employee.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!result.passwordMatch)
                {
                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ((App)Application.Current).CurrentUser = u;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static async Task<(bool found, bool passwordMatch, User? user)>
        TryLoginAsync(string phone, string passwordPlain)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Phone, phone);
            var user = await MongoDb.Users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
                return (false, false, null);

            var match = string.Equals(user.PasswordPlain, passwordPlain, StringComparison.Ordinal);
            return (true, match, user);
        }
        private string NormalizePhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";
            return new string(input.Where(char.IsDigit).ToArray());
        }
    }
}
