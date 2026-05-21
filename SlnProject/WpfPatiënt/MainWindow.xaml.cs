using System.Windows;
using WpfPatiënt.Views;

namespace WpfPatiënt;

// Hoofdvenster van de patiënt-app: zijmenu, header met profiel en Frame voor pages.
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Loaded in MainWindow.xaml: toont meteen header en StartPage in fraMain.
    // Patiënt-login zit (nog) niet in dit venster; Session wordt elders gevuld of blijft leeg.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LaadGebruikerInHeader();
        NavigeerNaarStart();
    }

    // Vult txtGebruikersnaam en imgProfiel uit Session; fallback "Patiënt" als naam ontbreekt.
    private void LaadGebruikerInHeader()
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

    // Interne navigatie: nieuwe StartPage-instantie in het Frame.
    private void NavigeerNaarStart()
    {
        fraMain.Navigate(new StartPage());
    }

    // Frame toont lijst/weergave van eigen afspraken (nog uit te werken in AfsprakenPage).
    private void NavigeerNaarAfspraken()
    {
        fraMain.Navigate(new AfsprakenPage());
    }

    // Frame toont formulier om een nieuwe afspraak aan te vragen.
    private void NavigeerNaarAfspraakMaken()
    {
        fraMain.Navigate(new AfspraakMakenPage());
    }

    // Frame toont profielpagina (gegevens bewerken, later).
    private void NavigeerNaarProfiel()
    {
        fraMain.Navigate(new ProfielPage());
    }

    // Zijmenu "Start".
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarStart();
    }

    // Zijmenu "Mijn afspraken".
    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarAfspraken();
    }

    // Zijmenu "Afspraak maken".
    private void BtnAfspraakMaken_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarAfspraakMaken();
    }

    // Extra menu-item profiel (indien aanwezig in XAML naast andere knoppen).
    private void BtnProfielMenu_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    // Klik op profielafbeelding in de header: zelfde bestemming als profiel in het menu.
    private void BtnProfiel_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    // Uitloggen: sessievelden op 0/null en applicatie afsluiten (geen terug naar login in deze app).
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Gebruikersnaam = null;
        Session.ProfielData = null;
        Session.GebruikerId = 0;
        Application.Current.Shutdown();
    }
}
