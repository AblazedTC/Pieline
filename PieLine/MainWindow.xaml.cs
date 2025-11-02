using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PieLine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Hide error message initially
            errorBorder.Visibility = Visibility.Collapsed;

            // Validate Full Name
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Error: Invalid name, please enter a valid name, then try again.");
                return;
            }

            // Validate Phone Number
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                ShowError("Error: Invalid phone number, please enter a valid phone number, then try again.");
                return;
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                ShowError("Error: Invalid email, please enter a valid email address, then try again.");
                return;
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowError("Error: Password cannot be empty, please enter a password, then try again.");
                return;
            }

            // Validate Password Match
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                ShowError("Error: Passwords do not match, please try again.");
                return;
            }

            // If all validations pass
            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Clear form
            ClearForm();
        }

        private void linkSignIn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigate to Sign In page", "Sign In", MessageBoxButton.OK, MessageBoxImage.Information);
            // TODO: Navigate to sign in page
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            errorBorder.Visibility = Visibility.Visible;
        }

        private void ClearForm()
        {
            txtFullName.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            errorBorder.Visibility = Visibility.Collapsed;
        }
    }
}