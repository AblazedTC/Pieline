using System.Windows;

namespace PieLine
{
    public partial class RegisterWindow : Window
    {
        private bool isUpdatingPhone = false;

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CommonHelpers.HandlePhoneTextChanged(PhoneTextBox, ref isUpdatingPhone);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            CommonHelpers.SetError(RegisterErrorTextBlock, null);

            string name = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phoneDigits = CommonHelpers.ExtractDigits(PhoneTextBox.Text);
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (password != confirm)
            {
                CommonHelpers.SetError(RegisterErrorTextBlock,
                    "Error: Password and confirmed password must match.");
                return;
            }

            if (!UserFile.TryAddUser(name, email, phoneDigits, password, out string error))
            {
                CommonHelpers.SetError(RegisterErrorTextBlock, error);
                return;
            }

            CommonHelpers.SetError(RegisterErrorTextBlock, null);
            var loginWindow = new LoginWindow();
            CommonHelpers.PersistantWindows(this, loginWindow);
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            CommonHelpers.PersistantWindows(this, loginWindow);
        }
    }
}
