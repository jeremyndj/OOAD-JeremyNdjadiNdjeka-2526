using System.Windows;

namespace WpfHelpdesk
{
    /// <summary>
    /// Startscherm: de gebruiker kiest tussen medewerker of helpdeskmedewerker.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnMedewerker_Click(object sender, RoutedEventArgs e)
        {
            MedewerkerWindow venster = new MedewerkerWindow();
            venster.Show();
            this.Close();
        }

        private void BtnHelpdesk_Click(object sender, RoutedEventArgs e)
        {
            HelpdeskWindow venster = new HelpdeskWindow();
            venster.Show();
            this.Close();
        }
    }
}
