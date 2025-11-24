using System.Windows;

namespace PieLine
{
    public partial class CompanyInformationWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        public CompanyInformationWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}