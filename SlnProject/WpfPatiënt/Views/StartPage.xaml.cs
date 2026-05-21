using System.Windows;
using System.Windows.Controls;

namespace WpfPatiënt.Views;

// =============================================================================
// StartPage — landingspagina na inloggen (via zijmenu Start)
// =============================================================================
// Geen database-aanroepen: alleen Session.Gebruikersnaam voor welkomsttekst.
// Snelkoppelingen naar AfsprakenPage en AfspraakMakenPage (naast het vaste zijmenu).
// NavigationService hoort bij fraMain in MainWindow; null-check voorkomt crash zonder host.
// =============================================================================
public partial class StartPage : Page
{
    public StartPage()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — personaliseer welkomsttekst
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        string strNaam = string.IsNullOrEmpty(Session.Gebruikersnaam) ? "patiënt" : Session.Gebruikersnaam;
        txtWelkom.Text = "Welkom, " + strNaam;
    }

    // Navigeert binnen hetzelfde Frame naar het overzicht van eigen afspraken.
    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new AfsprakenPage());
        }
    }

    // Navigeert naar het formulier om een nieuwe afspraak aan te vragen.
    private void BtnAfspraakMaken_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new AfspraakMakenPage());
        }
    }
}
