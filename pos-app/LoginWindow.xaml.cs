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
            var email = (UsernameBox.Text ?? "").Trim();
            var password = PasswordBox.Password ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username (email) and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = await TryLoginAsync(email, password);

                if (!result.found)
                {
                    MessageBox.Show("Email not found.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            TryLoginAsync(string email, string passwordPlain)
        {
            const string sql = @"
                SELECT id, full_name, phone, email, password_plain, user_type
                FROM users
                WHERE email = @e
                LIMIT 1;";

            await using var conn = await Db.OpenAsync("pieline_db");
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
                return (found: false, passwordMatch: false, user: null!);

            var id = rdr.GetInt32(0);
            var fullName = rdr.GetString(1);
            var phone = rdr.GetString(2);
            var storedEmail = rdr.GetString(3);
            var storedPwd = rdr.GetString(4);
            var userType = rdr.GetString(5);

            var match = string.Equals(storedPwd, passwordPlain, StringComparison.Ordinal);

            var user = new Data.User
            {
                Id = id,
                FullName = fullName,
                Phone = phone,
                Email = storedEmail,
                UserType = userType
            };

            return (found: true, passwordMatch: match, user);
        }
    }
}
