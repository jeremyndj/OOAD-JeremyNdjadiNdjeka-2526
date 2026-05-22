using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Debug;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfPatiënt.Views;

// =============================================================================
// ProfielPage — alle profielgegevens van de ingelogde patiënt (alleen-lezen)
// =============================================================================
// Data: PatientService.HaalOpId(Session.GebruikerId). E-mail is niet wijzigbaar (login).
// SQL- en profielfoto-fouten in txtFout; geen MessageBox voor technische fouten.
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
        // #region agent log
        DebugAgentLog.Write(
            "ProfielPage.xaml.cs:Page_Loaded",
            "entry",
            new { Session.GebruikerId, Session.Gebruikersnaam },
            "E");
        // #endregion

        if (Session.GebruikerId <= 0)
        {
            return;
        }

        VerbergFout();

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

            try
            {
                ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, patient.ProfielData);
            }
            catch (Exception exFoto)
            {
                ToonFout("Profielfoto tonen is mislukt: " + exFoto.Message);
            }
        }
        catch (Exception ex)
        {
            // #region agent log
            DebugAgentLog.Write(
                "ProfielPage.xaml.cs:Page_Loaded",
                "load failed",
                new { Session.GebruikerId, type = ex.GetType().Name, message = ex.Message },
                "E");
            // #endregion

            ToonFout("Profiel laden is mislukt: " + ex.Message);
        }
    }

    // Toont een fouttekst in het rode TextBlock boven het profielblok.
    private void ToonFout(string strMelding)
    {
        txtFout.Text = strMelding;
        txtFout.Visibility = Visibility.Visible;
    }

    // Verbergt txtFout bij het openen van de page.
    private void VerbergFout()
    {
        txtFout.Visibility = Visibility.Collapsed;
        txtFout.Text = string.Empty;
    }

    private void BtnBewerken_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new ProfielBewerkPage());
        }
    }
}
