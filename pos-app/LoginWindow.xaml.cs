using MySqlConnector;
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
            var username = (UsernameBox.Text ?? "").Trim();
            var password = PasswordBox.Password ?? "";
            var phone = new string(username.Where(char.IsDigit).ToArray());

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

        private static async Task<(bool found, bool passwordMatch, Data.User user)>
            TryLoginAsync(string phone, string passwordPlain)
        {
            const string sql = @"
                SELECT phone, email, full_name, password_plain, user_type
                FROM users
                WHERE phone = @p
                LIMIT 1;";

            await using var conn = await Db.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", phone);

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
                return (found: false, passwordMatch: false, user: null!);

            var dbPhone = rdr.GetString(rdr.GetOrdinal("phone"));
            var storedEmail = rdr.GetString(rdr.GetOrdinal("email"));
            var fullName = rdr.GetString(rdr.GetOrdinal("full_name"));
            var storedPwd = rdr.GetString(rdr.GetOrdinal("password_plain"));
            var userType = rdr.GetString(rdr.GetOrdinal("user_type"));

            var match = string.Equals(storedPwd, passwordPlain, StringComparison.Ordinal);

            var user = new Data.User
            {
                Phone = dbPhone,
                Email = storedEmail,
                FullName = fullName,
                UserType = userType
            };

            return (found: true, passwordMatch: match, user);
        }
    }
}