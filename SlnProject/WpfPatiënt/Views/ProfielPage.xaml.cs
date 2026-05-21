using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfPatiënt.Views;

// =============================================================================
// ProfielPage — alle profielgegevens van de ingelogde patiënt (alleen-lezen)
// =============================================================================
// Data: PatientService.HaalOpId(Session.GebruikerId). E-mail is niet wijzigbaar (login).
// Bewerken: navigatie naar ProfielBewerkPage.
// =============================================================================
public partial class ProfielPage : Page
{
    private readonly PatientService _svcPatient = new PatientService();

    public ProfielPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (Session.GebruikerId <= 0)
        {
            return;
        }

        try
        {
            Patient? patient = _svcPatient.HaalOpId(Session.GebruikerId);
            if (patient == null)
            {
                txtNaam.Text = "Profiel niet gevonden.";
                return;
            }

            txtNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            txtEmail.Text = patient.Email;
            txtGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "—" : patient.Gsm;
            txtGeboortedatum.Text = patient.Geboortedatum.ToString("d MMMM yyyy", new CultureInfo("nl-BE"));
            txtGeslacht.Text = ProfielWeergaveHelper.VertaalGeslacht(patient.Geslacht);
            txtNotificaties.Text = ProfielWeergaveHelper.VertaalNotificatie(patient.NotificatieKeuze);

            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, patient.ProfielData);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Profiel laden is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void BtnBewerken_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new ProfielBewerkPage());
        }
    }
}
