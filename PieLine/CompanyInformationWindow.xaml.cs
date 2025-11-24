using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace PieLine
{
    public partial class CompanyInformationWindow : Window
    {
        public CompanyInformationWindow()
        {
            InitializeComponent();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Open MainWindow and close this one
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Open the URL in the default browser
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
