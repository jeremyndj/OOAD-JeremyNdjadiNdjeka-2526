using System.Windows;
using System.Windows.Controls;

namespace WpfDokter.Views;

// =============================================================================
// StartPage — landingspagina na inloggen
// =============================================================================
// Geen database-aanroepen: alleen Session.Gebruikersnaam voor welkomsttekst.
// Biedt snelkoppelingen naar AfsprakenPage en PatientenPage (naast het vaste zijmenu).
// NavigationService hoort bij het Frame in MainWindow; null-check voorkomt crash bij ontbrekende host.
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
    // Als Gebruikersnaam leeg is (zou niet mogen na login), tonen we generiek "dokter".
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        string strNaam = string.IsNullOrEmpty(Session.Gebruikersnaam) ? "dokter" : Session.Gebruikersnaam;
        txtWelkom.Text = "Welkom, " + strNaam;
    }

    // Navigeert binnen hetzelfde Frame naar de afsprakenkalender van de ingelogde dokter.
    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new AfsprakenPage());
        }
    }

    // Navigeert naar het patiëntenoverzicht (contactkaarten + zoeken).
    private void BtnPatienten_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientenPage());
        }
    }
}
