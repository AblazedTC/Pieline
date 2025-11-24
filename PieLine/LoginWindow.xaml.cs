using System;
using System.Windows;

namespace PieLine
{
    public partial class LoginWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        private bool isUpdatingPhone = false;

        private int failedLoginAttempts = 0;
        private DateTime? lockoutUntil = null;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoginButton.IsEnabled = false;
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CommonHelpers.HandlePhoneTextChanged(PhoneTextBox, ref isUpdatingPhone);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateLoginButtonState();
        }

        private void StartLockout()
        {
            lockoutUntil = DateTime.Now.AddMinutes(10);
            CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock,
                "Error: Maximum sign in attempts reached, please try again in 10 minutes.");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (lockoutUntil.HasValue && DateTime.Now < lockoutUntil.Value)
            {
                CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock,
                    "Error: Maximum sign in attempts reached, please try again in 10 minutes.");
                return;
            }

            CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock, null);

            string phoneDigits = CommonHelpers.ExtractDigits(PhoneTextBox.Text);
            string password = PasswordBox.Password;

            var user = UserFile.FindUserByPhone(phoneDigits);
            if (user == null)
            {
                failedLoginAttempts++;
                if (failedLoginAttempts > 10)
                {
                    StartLockout();
                    return;
                }

                CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock,
                    "Error: Invalid phone number, please enter a valid phone number then try again.");
                return;
            }

            if (user.Password != password)
            {
                failedLoginAttempts++;
                if (failedLoginAttempts > 10)
                {
                    StartLockout();
                    return;
                }

                CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock,
                    "Error: Invalid password, please enter a valid password then try again.");
                return;
            }

            failedLoginAttempts = 0;
            lockoutUntil = null;
            CommonHelpers.SetError(LoginErrorBorder, LoginErrorTextBlock, null);

            var main = new MainWindow();
            CommonHelpers.PersistantWindows(this, main);
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var resetWindow = new PasswordResetWindow();
            CommonHelpers.PersistantWindows(this, resetWindow);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            CommonHelpers.PersistantWindows(this, registerWindow);
        }

        private void UpdateLoginButtonState()
        {
            bool hasPhone = !string.IsNullOrWhiteSpace(PhoneTextBox.Text);
            bool hasPassword = !string.IsNullOrWhiteSpace(PasswordBox.Password);

            LoginButton.IsEnabled = hasPhone && hasPassword;
        }

    }
}
