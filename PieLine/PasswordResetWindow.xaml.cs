using System.Windows;

namespace PieLine
{
    public partial class PasswordResetWindow : Window
    {
        private bool isUpdatingPhone = false;

        public PasswordResetWindow()
        {
            InitializeComponent();
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CommonHelpers.HandlePhoneTextChanged(PhoneTextBox, ref isUpdatingPhone);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            CommonHelpers.SetError(ResetErrorTextBlock, null);

            string phoneDigits = CommonHelpers.ExtractDigits(PhoneTextBox.Text);
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (password != confirm)
            {
                CommonHelpers.SetError(ResetErrorTextBlock,
                    "Error: Password and confirmed password must match.");
                return;
            }

            if (!UserFile.TryResetPassword(phoneDigits, email, password, out string error))
            {
                CommonHelpers.SetError(ResetErrorTextBlock, error);
                return;
            }

            CommonHelpers.SetError(ResetErrorTextBlock, null);
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
