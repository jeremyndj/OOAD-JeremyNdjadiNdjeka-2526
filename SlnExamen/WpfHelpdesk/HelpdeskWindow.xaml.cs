using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk
{
    /// <summary>
    /// Venster voor helpdeskmedewerkers: tickets raadplegen, filteren,
    /// nieuwe tickets registreren en tickets afsluiten.
    /// Geen data binding; ListBox wordt handmatig gevuld.
    /// </summary>
    public partial class HelpdeskWindow : Window
    {
        /// <summary>
        /// Parallelle lijst met ticket-ids die overeenkomen met de ListBox-items.
        /// </summary>
        private List<int> ticketIds;

        /// <summary>
        /// Parallelle lijst met medewerkers die overeenkomen met de melder-ComboBox.
        /// </summary>
        private List<Medewerker> melderLijst;

        public HelpdeskWindow()
        {
            InitializeComponent();
            ticketIds = new List<int>();
            melderLijst = new List<Medewerker>();
            VulFilterComboBoxes();
            VulNieuwTicketComboBoxes();
            VulMelderComboBox();
            VernieuwTicketLijst();
        }

        /// <summary>
        /// Vult de filter-ComboBoxes handmatig.
        /// </summary>
        private void VulFilterComboBoxes()
        {
            cmbFilterPrioriteit.Items.Clear();
            cmbFilterPrioriteit.Items.Add("Alle");
            cmbFilterPrioriteit.Items.Add("Laag");
            cmbFilterPrioriteit.Items.Add("Normaal");
            cmbFilterPrioriteit.Items.Add("Hoog");
            cmbFilterPrioriteit.SelectedIndex = 0;

            cmbFilterType.Items.Clear();
            cmbFilterType.Items.Add("Alle");
            cmbFilterType.Items.Add("Hardware");
            cmbFilterType.Items.Add("Software");
            cmbFilterType.SelectedIndex = 0;
        }

        /// <summary>
        /// Vult de ComboBoxes voor het nieuw-ticket-formulier.
        /// </summary>
        private void VulNieuwTicketComboBoxes()
        {
            cmbNieuwType.Items.Clear();
            cmbNieuwType.Items.Add("Hardware");
            cmbNieuwType.Items.Add("Software");
            cmbNieuwType.SelectedIndex = 0;

            cmbNieuwPrioriteit.Items.Clear();
            cmbNieuwPrioriteit.Items.Add("Laag");
            cmbNieuwPrioriteit.Items.Add("Normaal");
            cmbNieuwPrioriteit.Items.Add("Hoog");
            cmbNieuwPrioriteit.SelectedIndex = 1;
        }

        /// <summary>
        /// Vult de melder-ComboBox met unieke medewerkers uit het CSV-bestand.
        /// </summary>
        private void VulMelderComboBox()
        {
            cmbMelder.Items.Clear();
            melderLijst.Clear();

            List<Medewerker> medewerkers = App.Beheer.GetMedewerkers();

            foreach (Medewerker medewerker in medewerkers)
            {
                cmbMelder.Items.Add(medewerker.ToString());
                melderLijst.Add(medewerker);
            }

            if (cmbMelder.Items.Count > 0)
            {
                cmbMelder.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Vernieuwt de ListBox met gefilterde tickets. Geen data binding.
        /// </summary>
        private void VernieuwTicketLijst()
        {
            lstTickets.Items.Clear();
            ticketIds.Clear();
            txtDetails.Text = "";

            TicketPrioriteit? prioriteitFilter = BepaalPrioriteitFilter();
            string typeFilter = BepaalTypeFilter();
            bool? statusFilter = BepaalStatusFilter();
            string zoek = txtZoeken.Text.Trim();

            List<Ticket> tickets = App.Beheer.FilterTickets(
                prioriteitFilter, typeFilter, statusFilter, zoek);

            foreach (Ticket ticket in tickets)
            {
                lstTickets.Items.Add(ticket.ToString());
                ticketIds.Add(ticket.Id);
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            VernieuwTicketLijst();
        }

        private void FilterGewijzigd(object sender, RoutedEventArgs e)
        {
            VernieuwTicketLijst();
        }

        /// <summary>
        /// Toont de detailinfo van het geselecteerde ticket via GeefInfo().
        /// </summary>
        private void LstTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstTickets.SelectedIndex;
            if (index < 0 || index >= ticketIds.Count)
            {
                txtDetails.Text = "";
                return;
            }

            int ticketId = ticketIds[index];
            Ticket ticket = App.Beheer.GetTicketOpId(ticketId);

            if (ticket != null)
            {
                txtDetails.Text = ticket.GeefInfo();
            }
        }

        private void CmbNieuwType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNieuwType.SelectedItem == null)
            {
                return;
            }

            string gekozen = cmbNieuwType.SelectedItem.ToString();
            if (gekozen == "Hardware")
            {
                lblNieuwExtra.Content = "Toestel:";
            }
            else
            {
                lblNieuwExtra.Content = "Applicatie:";
            }
        }

        private void BtnNieuwRegistreer_Click(object sender, RoutedEventArgs e)
        {
            string fout = ValideerNieuwTicket();
            if (!string.IsNullOrEmpty(fout))
            {
                ToonFoutmelding(fout);
                return;
            }

            WisFoutmelding();

            try
            {
                string titel = txtNieuwTitel.Text.Trim();
                string extra = txtNieuwExtra.Text.Trim();
                string type = cmbNieuwType.SelectedItem.ToString();
                string prioriteitTekst = cmbNieuwPrioriteit.SelectedItem.ToString();
                int melderIndex = cmbMelder.SelectedIndex;
                Medewerker melder = melderLijst[melderIndex];
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

                WisNieuwVelden();
                VulMelderComboBox();
                VernieuwTicketLijst();
            }
            catch (System.Exception ex)
            {
                ToonFoutmelding(ex.Message);
            }
        }

        /// <summary>
        /// Valideert de invoer voor een nieuw ticket. Geeft een foutboodschap terug of een lege string.
        /// </summary>
        private string ValideerNieuwTicket()
        {
            if (string.IsNullOrWhiteSpace(txtNieuwTitel.Text))
            {
                return "Titel is verplicht.";
            }

            if (cmbMelder.SelectedIndex < 0)
            {
                return "Selecteer een melder.";
            }

            if (cmbNieuwType.SelectedItem == null)
            {
                return "Selecteer een type (Hardware of Software).";
            }

            if (cmbNieuwPrioriteit.SelectedItem == null)
            {
                return "Selecteer een prioriteit.";
            }

            string type = cmbNieuwType.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(txtNieuwExtra.Text))
            {
                if (type == "Hardware")
                {
                    return "Toestel is verplicht voor een hardwareticket.";
                }
                return "Applicatie is verplicht voor een softwareticket.";
            }

            return "";
        }

        /// <summary>
        /// Toont een rode foutmelding in de UI (geen MessageBox).
        /// </summary>
        private void ToonFoutmelding(string bericht)
        {
            txtFoutmelding.Text = bericht;
            txtFoutmelding.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Verbergt de foutmelding.
        /// </summary>
        private void WisFoutmelding()
        {
            txtFoutmelding.Text = "";
            txtFoutmelding.Visibility = Visibility.Collapsed;
        }

        private void BtnSluitAf_Click(object sender, RoutedEventArgs e)
        {
            int index = lstTickets.SelectedIndex;
            if (index < 0)
            {
                ToonFoutmelding("Selecteer eerst een ticket om af te sluiten.");
                return;
            }

            WisFoutmelding();

            try
            {
                int ticketId = ticketIds[index];
                App.Beheer.SluitTicketAf(ticketId);
                VernieuwTicketLijst();
            }
            catch (System.Exception ex)
            {
                ToonFoutmelding(ex.Message);
            }
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private TicketPrioriteit? BepaalPrioriteitFilter()
        {
            if (cmbFilterPrioriteit.SelectedItem == null)
            {
                return null;
            }

            string gekozen = cmbFilterPrioriteit.SelectedItem.ToString();
            if (gekozen == "Laag")
            {
                return TicketPrioriteit.Laag;
            }
            if (gekozen == "Normaal")
            {
                return TicketPrioriteit.Normaal;
            }
            if (gekozen == "Hoog")
            {
                return TicketPrioriteit.Hoog;
            }
            return null;
        }

        private string BepaalTypeFilter()
        {
            if (cmbFilterType.SelectedItem == null)
            {
                return "";
            }

            string gekozen = cmbFilterType.SelectedItem.ToString();
            if (gekozen == "Alle")
            {
                return "";
            }
            return gekozen;
        }

        /// <summary>
        /// Als de checkbox aangevinkt is, toon alleen open tickets.
        /// </summary>
        private bool? BepaalStatusFilter()
        {
            if (chkAlleenOpen.IsChecked == true)
            {
                return false;
            }
            return null;
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

        private void WisNieuwVelden()
        {
            txtNieuwTitel.Text = "";
            txtNieuwExtra.Text = "";
        }
    }
}
