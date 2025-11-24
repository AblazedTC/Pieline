using System.Windows;

namespace PieLine
{
    public partial class PasswordResetWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        private bool isUpdatingPhone = false;

        public PasswordResetWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CommonHelpers.HandlePhoneTextChanged(PhoneTextBox, ref isUpdatingPhone);
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateResetButtonState();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateResetButtonState();
        }

        private void ConfirmedPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateResetButtonState();
        }

        private void UpdateResetButtonState()
        {
            bool hasPhone = !string.IsNullOrWhiteSpace(PhoneTextBox.Text);
            bool hasEmail = !string.IsNullOrWhiteSpace(EmailTextBox.Text);
            bool hasPassword = !string.IsNullOrWhiteSpace(PasswordBox.Password);
            bool hasConfirmedPassword = !string.IsNullOrWhiteSpace(ConfirmedPasswordBox.Password);

            ResetButton.IsEnabled = hasPhone && hasPassword && hasEmail && hasConfirmedPassword;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            CommonHelpers.SetError(ResetErrorBorder, ResetErrorTextBlock, null);

            string phoneDigits = CommonHelpers.ExtractDigits(PhoneTextBox.Text);
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmedPasswordBox.Password;

            if (password != confirm)
            {
                CommonHelpers.SetError(ResetErrorBorder, ResetErrorTextBlock,
                    "Error: Password and confirmed password must match.");
                return;
            }

            if (!UserFile.TryResetPassword(phoneDigits, email, password, out string error))
            {
                CommonHelpers.SetError(ResetErrorBorder, ResetErrorTextBlock, error);
                return;
            }

            CommonHelpers.SetError(ResetErrorBorder, ResetErrorTextBlock, null);
            var loginWindow = new LoginWindow();
            CommonHelpers.PersistantWindows(this, loginWindow);
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            CommonHelpers.PersistantWindows(this, registerWindow);
        }
    }
}