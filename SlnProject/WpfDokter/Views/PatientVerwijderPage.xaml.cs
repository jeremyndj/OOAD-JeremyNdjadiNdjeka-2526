using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// =============================================================================
// PatientVerwijderPage — patiënt definitief verwijderen met bevestiging
// =============================================================================
// Navigatie: PatientenPage → verwijder-icoon op kaart → deze Page met patiënt-id.
// Bevestiging via MessageBox (zoals afspraak annuleren); technische fouten in txtFout.
// PatientService.VerwijderInclusiefAfspraken: eerst DELETE op Afspraak, dan DELETE op Patient.
// =============================================================================
public partial class PatientVerwijderPage : Page
{
    private readonly int _iPatientId;
    private readonly PatientService _svcPatient = new PatientService();
    // Naam voor de bevestigingsvraag; leeg als patiënt niet geladen kon worden.
    private string _strPatientNaam = string.Empty;

    public PatientVerwijderPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — patiënt ophalen en uitlegtekst tonen
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Patient? patient = _svcPatient.HaalOpId(_iPatientId);
            if (patient == null)
            {
                txtUitleg.Text = "De patiënt kon niet worden gevonden.";
                btnVerwijderen.IsEnabled = false;
                return;
            }

            _strPatientNaam = patient.Voornaam + " " + patient.Achternaam;
            txtUitleg.Text =
                "U staat op het punt om " + _strPatientNaam + " te verwijderen. Deze actie kan niet ongedaan worden gemaakt.";
        }
        catch (Exception ex)
        {
            txtUitleg.Text = "Gegevens laden is mislukt.";
            ToonFout(ex.Message);
            btnVerwijderen.IsEnabled = false;
        }
    }

    // -------------------------------------------------------------------------
    // BtnVerwijderen_Click — MessageBox-bevestiging, dan verwijderen in database
    // -------------------------------------------------------------------------
    private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        string strVraag =
            "Weet u zeker dat u " + _strPatientNaam + " permanent wilt verwijderen?\n\n" +
            "Alle gekoppelde afspraken worden ook verwijderd.";

        MessageBoxResult resultaat = MessageBox.Show(
            strVraag,
            "Patiënt verwijderen",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (resultaat != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            bool bGelukt = _svcPatient.VerwijderInclusiefAfspraken(_iPatientId);
            if (!bGelukt)
            {
                ToonFout("De patiënt kon niet worden verwijderd.");
                return;
            }

            if (NavigationService != null)
            {
                NavigationService.Navigate(new PatientenPage());
            }
        }
        catch (Exception ex)
        {
            ToonFout("Verwijderen is mislukt: " + ex.Message);
        }
    }

    // Annuleren zonder verwijdering: terug naar patiëntenoverzicht.
    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientenPage());
        }
    }

    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientenPage());
        }
    }

    private void ToonFout(string strMelding)
    {
        txtFout.Text = strMelding;
        txtFout.Visibility = Visibility.Visible;
    }

    private void VerbergFout()
    {
        txtFout.Visibility = Visibility.Collapsed;
        txtFout.Text = string.Empty;
    }
}
