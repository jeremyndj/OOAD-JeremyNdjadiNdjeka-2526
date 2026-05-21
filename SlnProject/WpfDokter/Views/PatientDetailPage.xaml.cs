using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// =============================================================================
// PatientDetailPage — alleen-lezen detail van één patiënt
// =============================================================================
// Het patiënt-id komt via de constructor (doorgegeven bij Navigate vanaf een kaart).
// Geen bewerkingsvelden: alleen tonen wat PatientService.HaalOpId uit SQL haalt.
// Geslacht in de DB is int (0/1/2); in het model als string → VertaalGeslacht voor de UI.
// =============================================================================
public partial class PatientDetailPage : Page
{
    // Vast id voor deze Page-instantie; verandert niet na constructie.
    private readonly int _iPatientId;
    private readonly PatientService _svcPatient = new PatientService();

    public PatientDetailPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — eenmalig gegevens laden en controls vullen
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Patient? patient = _svcPatient.HaalOpId(_iPatientId);
            if (patient == null)
            {
                // Id bestaat niet (verwijderd of foutieve navigatie): duidelijke melding, geen exception.
                txtTitel.Text = "Patiënt niet gevonden";
                txtNaam.Text = "Er is geen patiënt met dit id.";
                txtEmail.Text = string.Empty;
                txtGsm.Text = string.Empty;
                txtGeboortedatum.Text = string.Empty;
                txtGeslacht.Text = string.Empty;
                txtNotificaties.Text = string.Empty;
                return;
            }

            txtTitel.Text = "Patiënt — " + patient.Voornaam + " " + patient.Achternaam;
            txtNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            txtEmail.Text = patient.Email;
            txtGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "—" : patient.Gsm;
            txtGeboortedatum.Text = patient.Geboortedatum.ToString("d MMMM yyyy", new CultureInfo("nl-BE"));
            txtGeslacht.Text = VertaalGeslacht(patient.Geslacht);
            txtNotificaties.Text = patient.NotificatieKeuze.ToString();

            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, patient.ProfielData);
        }
        catch (Exception ex)
        {
            // Technische fout (SQL/verbinding): MessageBox, geen inline txtFout op deze page.
            MessageBox.Show(
                "Gegevens laden is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // -------------------------------------------------------------------------
    // VertaalGeslacht — mapping database → leesbare tekst
    // -------------------------------------------------------------------------
    // Conventie in seed-data: 0 = Vrouw, 1 = Man, 2 = Anders.
    private static string VertaalGeslacht(string strCode)
    {
        if (strCode == "0")
        {
            return "Vrouw";
        }

        if (strCode == "1")
        {
            return "Man";
        }

        if (strCode == "2")
        {
            return "Anders";
        }

        return "Code " + strCode;
    }

    // Terug naar PatientenPage zonder wijzigingen op te slaan.
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientenPage());
        }
    }
}
