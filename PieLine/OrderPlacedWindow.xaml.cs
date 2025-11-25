using System.Windows;

namespace PieLine
{
    public partial class OrderPlacedWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        public OrderPlacedWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ViewReceipt_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            var info = new CompanyInformationWindow();
            info.Show();
            this.Close();
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var info = new CompanyInformationWindow();
            info.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Close();
        }
        
    }
}
