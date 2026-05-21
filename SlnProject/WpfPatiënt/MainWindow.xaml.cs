using System.Windows;
using WpfPatiënt.Views;

namespace WpfPatiënt;

// =============================================================================
// MainWindow — shell van de patiënt-applicatie
// =============================================================================
// Het Frame (fraMain) toont telkens een andere Page; geen apart LoginWindow.
// Links: zijmenu (Start, Mijn afspraken, Afspraak maken, Profiel, Uitloggen).
// Boven: header met naam en profielfoto van de ingelogde patiënt (Session).
// Geen SQL in dit bestand — alleen navigatie en sessie-gedrag in de UI.
// =============================================================================
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Window_Loaded — gekoppeld aan Loaded in MainWindow.xaml
    // -------------------------------------------------------------------------
    // GebruikerId == 0: menu uit (zichtbaar maar disabled), LoginPage in fraMain.
    // GebruikerId > 0: menu aan, header vullen, standaard Mijn afspraken.
    // -------------------------------------------------------------------------
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (Session.GebruikerId == 0)
        {
            ZetMenuIngelogd(false);
            WisGebruikerInHeader();
            fraMain.Navigate(new LoginPage());
        }
        else
        {
            ZetMenuIngelogd(true);
            LaadGebruikerInHeader();
            NavigeerNaarAfspraken();
        }
    }

    // -------------------------------------------------------------------------
    // NaLogin — publiek, aangeroepen door LoginPage na geslaagde LoginService.LoginPatiënt
    // -------------------------------------------------------------------------
    public void NaLogin()
    {
        ZetMenuIngelogd(true);
        LaadGebruikerInHeader();
        NavigeerNaarAfspraken();
    }

    // -------------------------------------------------------------------------
    // ZetMenuIngelogd — zijmenu + profielknop in header aan/uit
    // -------------------------------------------------------------------------
    // IsEnabled = false: knoppen blijven zichtbaar tijdens login, niet klikbaar.
    // -------------------------------------------------------------------------
    private void ZetMenuIngelogd(bool bIngelogd)
    {
        btnStart.IsEnabled = bIngelogd;
        btnAfspraken.IsEnabled = bIngelogd;
        btnAfspraakMaken.IsEnabled = bIngelogd;
        btnProfielMenu.IsEnabled = bIngelogd;
        btnLogout.IsEnabled = bIngelogd;
        btnProfiel.IsEnabled = bIngelogd;
    }

    // -------------------------------------------------------------------------
    // WisGebruikerInHeader — anonieme header (vóór login / na uitloggen)
    // -------------------------------------------------------------------------
    private void WisGebruikerInHeader()
    {
        txtGebruikersnaam.Text = "Niet ingelogd";
        ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, null);
    }

    // -------------------------------------------------------------------------
    // LaadGebruikerInHeader — ingelogde patiënt tonen uit Session
    // -------------------------------------------------------------------------
    public void LaadGebruikerInHeader()
    {
        if (!string.IsNullOrEmpty(Session.Gebruikersnaam))
        {
            txtGebruikersnaam.Text = Session.Gebruikersnaam;
        }
        else
        {
            txtGebruikersnaam.Text = "Patiënt";
        }

        ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, Session.ProfielData);
    }

    // -------------------------------------------------------------------------
    // Navigatie-helpers — elke methode vervangt de huidige Page in fraMain
    // -------------------------------------------------------------------------
    private void NavigeerNaarStart()
    {
        fraMain.Navigate(new StartPage());
    }

    private void NavigeerNaarAfspraken()
    {
        fraMain.Navigate(new AfsprakenPage());
    }

    private void NavigeerNaarAfspraakMaken()
    {
        fraMain.Navigate(new AfspraakMakenPage());
    }

    private void NavigeerNaarProfiel()
    {
        fraMain.Navigate(new ProfielPage());
    }

    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarStart();
    }

    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarAfspraken();
    }

    private void BtnAfspraakMaken_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarAfspraakMaken();
    }

    private void BtnProfielMenu_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    private void BtnProfiel_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    // Uitloggen: sessie wissen en UI terug naar login (zelfde flow als opstart zonder sessie).
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Wis();
        ZetMenuIngelogd(false);
        WisGebruikerInHeader();
        fraMain.Navigate(new LoginPage());
    }
}
