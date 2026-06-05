using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk
{
    /// <summary>
    /// Venster voor medewerkers om een nieuw ticket aan te melden.
    /// Alle data wordt via TicketBeheer verwerkt; geen CSV in code-behind.
    /// </summary>
    public partial class MedewerkerWindow : Window
    {
        public MedewerkerWindow()
        {
            InitializeComponent();
            VulComboBoxes();
        }

        private void VulComboBoxes()
        {
            cmbType.Items.Clear();
            cmbType.Items.Add("Hardware");
            cmbType.Items.Add("Software");
            cmbType.SelectedIndex = 0;

            cmbPrioriteit.Items.Clear();
            cmbPrioriteit.Items.Add("Laag");
            cmbPrioriteit.Items.Add("Normaal");
            cmbPrioriteit.Items.Add("Hoog");
            cmbPrioriteit.SelectedIndex = 1;
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbType.SelectedItem == null)
            {
                return;
            }

            string gekozen = cmbType.SelectedItem.ToString();
            if (gekozen == "Hardware")
            {
                lblExtra.Content = "Toestel:";
            }
            else
            {
                lblExtra.Content = "Applicatie:";
            }
        }

        private void BtnRegistreer_Click(object sender, RoutedEventArgs e)
        {
            string fout = ValideerInvoer();
            if (!string.IsNullOrEmpty(fout))
            {
                ToonFoutmelding(fout);
                return;
            }

            WisFoutmelding();

            try
            {
                string titel = txtTitel.Text.Trim();
                string extra = txtExtra.Text.Trim();
                string voornaam = txtVoornaam.Text.Trim();
                string achternaam = txtAchternaam.Text.Trim();
                string melderId = txtMelderId.Text.Trim();
                string type = cmbType.SelectedItem.ToString();
                string prioriteitTekst = cmbPrioriteit.SelectedItem.ToString();

                Medewerker melder = new Medewerker(melderId, voornaam, achternaam);
                TicketPrioriteit prioriteit = ParsePrioriteit(prioriteitTekst);

                Ticket ticket = null;
                if (type == "Hardware")
                {
                    HardwareTicket hw = new HardwareTicket();
                    hw.Toestel = extra;
                    ticket = hw;
                }
                else
                {
                    SoftwareTicket sw = new SoftwareTicket();
                    sw.Applicatie = extra;
                    ticket = sw;
                }

                ticket.Titel = titel;
                ticket.Melder = melder;
                ticket.Prioriteit = prioriteit;

                App.Beheer.VoegTicketToe(ticket);

                ToonSuccesmelding("Ticket #" + ticket.Id + " is geregistreerd.");
                WisVelden();
            }
            catch (System.Exception ex)
            {
                ToonFoutmelding(ex.Message);
            }
        }

        private string ValideerInvoer()
        {
            if (string.IsNullOrWhiteSpace(txtTitel.Text))
            {
                return "Titel is verplicht.";
            }

            if (string.IsNullOrWhiteSpace(txtVoornaam.Text))
            {
                return "Voornaam is verplicht.";
            }

            if (string.IsNullOrWhiteSpace(txtAchternaam.Text))
            {
                return "Achternaam is verplicht.";
            }

            if (string.IsNullOrWhiteSpace(txtMelderId.Text))
            {
                return "Medewerker-ID is verplicht.";
            }

            if (cmbType.SelectedItem == null)
            {
                return "Selecteer een type (Hardware of Software).";
            }

            string type = cmbType.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(txtExtra.Text))
            {
                if (type == "Hardware")
                {
                    return "Toestel is verplicht voor een hardwareticket.";
                }
                return "Applicatie is verplicht voor een softwareticket.";
            }

            return "";
        }

        private void ToonFoutmelding(string bericht)
        {
            txtSuccesmelding.Visibility = Visibility.Collapsed;
            txtFoutmelding.Text = bericht;
            txtFoutmelding.Visibility = Visibility.Visible;
        }

        private void ToonSuccesmelding(string bericht)
        {
            txtFoutmelding.Visibility = Visibility.Collapsed;
            txtSuccesmelding.Text = bericht;
            txtSuccesmelding.Visibility = Visibility.Visible;
        }

        private void WisFoutmelding()
        {
            txtFoutmelding.Text = "";
            txtFoutmelding.Visibility = Visibility.Collapsed;
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void WisVelden()
        {
            txtTitel.Text = "";
            txtExtra.Text = "";
            txtVoornaam.Text = "";
            txtAchternaam.Text = "";
            txtMelderId.Text = "";
        }

        private TicketPrioriteit ParsePrioriteit(string tekst)
        {
            if (tekst == "Laag")
            {
                return TicketPrioriteit.Laag;
            }
            if (tekst == "Hoog")
            {
                return TicketPrioriteit.Hoog;
            }
            return TicketPrioriteit.Normaal;
        }
    }
}
