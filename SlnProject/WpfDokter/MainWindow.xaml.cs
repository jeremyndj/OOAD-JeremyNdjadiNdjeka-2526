using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Debug;
using WpfDokter.Views;

namespace WpfDokter;

// =============================================================================
// MainWindow — shell van de dokter-applicatie
// =============================================================================
// Dit venster verandert zelden van inhoud: het Frame (fraMain) toont telkens een andere Page.
// Links: vast zijmenu (Start, Afspraken, Patiënten, Uitloggen).
// Boven: header met naam en profielfoto van de ingelogde dokter (Session).
// Er is bewust geen apart LoginWindow: login is een Page in hetzelfde Frame.
// =============================================================================
public partial class MainWindow : Window
{
    // Constructor: alleen XAML laden. Navigatie en sessie starten pas in Window_Loaded.
    public MainWindow()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Window_Loaded — gekoppeld aan Loaded in MainWindow.xaml
    // -------------------------------------------------------------------------
    // Bepaalt het startscherm op basis van Session.GebruikerId:
    // - 0  → nog niet ingelogd: menu uit, header leeg, LoginPage in fraMain
    // - >0 → sessie actief (normaal na login): menu aan, header vullen, PatiëntenPage
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
            NavigeerNaarPatienten();
        }
    }

    // -------------------------------------------------------------------------
    // NaLogin — publiek, aangeroepen door LoginPage na geslaagde LoginService.Login
    // -------------------------------------------------------------------------
    // Zet de UI in dezelfde staat als een opstart met bestaande sessie:
    // menu klikbaar, header met doktergegevens, standaardnavigatie naar patiëntenoverzicht.
    // -------------------------------------------------------------------------
    public void NaLogin()
    {
        // #region agent log
        DebugAgentLog.Write(
            "MainWindow.xaml.cs:NaLogin",
            "after login navigation",
            new { Session.GebruikerId, Session.Gebruikersnaam },
            "D");
        // #endregion

        ZetMenuIngelogd(true);
        LaadGebruikerInHeader();
        NavigeerNaarPatienten();
    }

    // -------------------------------------------------------------------------
    // ZetMenuIngelogd — zijmenu + profielknop in header aan/uit
    // -------------------------------------------------------------------------
    // IsEnabled = false: knoppen blijven ZICHTBAAR tijdens login (UX-eis), maar niet klikbaar.
    // IsEnabled = true: normale navigatie na inloggen.
    // -------------------------------------------------------------------------
    private void ZetMenuIngelogd(bool bIngelogd)
    {
        btnStart.IsEnabled = bIngelogd;
        btnAfspraken.IsEnabled = bIngelogd;
        btnPatienten.IsEnabled = bIngelogd;
        btnLogout.IsEnabled = bIngelogd;
        btnProfiel.IsEnabled = bIngelogd;
    }

    // -------------------------------------------------------------------------
    // WisGebruikerInHeader — anonieme header (vóór login / na uitloggen)
    // -------------------------------------------------------------------------
    private void WisGebruikerInHeader()
    {
        txtGebruikersnaam.Text = "Niet ingelogd";
        LaadHeaderProfielAfbeelding(null);
    }

    // -------------------------------------------------------------------------
    // LaadGebruikerInHeader — ingelogde dokter tonen uit Session
    // -------------------------------------------------------------------------
    // Session wordt gevuld door Session.VulVanDokter na login (id, naam, profielfoto-bytes).
    // -------------------------------------------------------------------------
    public void LaadGebruikerInHeader()
    {
        if (!string.IsNullOrEmpty(Session.Gebruikersnaam))
        {
            txtGebruikersnaam.Text = Session.Gebruikersnaam;
        }
        else
        {
            txtGebruikersnaam.Text = "Dokter";
        }

        LaadHeaderProfielAfbeelding(Session.ProfielData);
    }

    // -------------------------------------------------------------------------
    // LaadHeaderProfielAfbeelding — profielfoto in header (try-catch, geen txtFout)
    // -------------------------------------------------------------------------
    private void LaadHeaderProfielAfbeelding(byte[]? arrProfielData)
    {
        try
        {
            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, arrProfielData);
        }
        catch (Exception)
        {
            imgProfiel.Source = null;
        }
    }

    // -------------------------------------------------------------------------
    // Navigatie-helpers — elke methode vervangt de huidige Page in fraMain
    // -------------------------------------------------------------------------
    // Nieuwe Page-instantie per navigatie (geen cache); geschiedenis via Frame-journal indien nodig.
    private void NavigeerNaarStart()
    {
        fraMain.Navigate(new StartPage());
    }

    private void NavigeerNaarAfspraken()
    {
        fraMain.Navigate(new AfsprakenPage());
    }

    private void NavigeerNaarPatienten()
    {
        fraMain.Navigate(new PatientenPage());
    }

    private void NavigeerNaarProfiel()
    {
        fraMain.Navigate(new ProfielPage());
    }

    // -------------------------------------------------------------------------
    // Click-handlers zijmenu en header — delegeren naar Navigeer*-methodes
    // -------------------------------------------------------------------------
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarStart();
    }

    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarAfspraken();
    }

    private void BtnPatienten_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarPatienten();
    }

    private void BtnProfiel_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    // Uitloggen: geheugen-sessie wissen en UI terug naar login-scherm (zelfde flow als opstart zonder sessie).
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Wis();
        ZetMenuIngelogd(false);
        WisGebruikerInHeader();
        fraMain.Navigate(new LoginPage());
    }
}
