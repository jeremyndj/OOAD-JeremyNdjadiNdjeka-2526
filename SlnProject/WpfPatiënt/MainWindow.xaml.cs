using System.Windows;
using WpfPatiënt.Views;

namespace WpfPatiënt;

// Hoofdvenster: zijmenu, header en Frame voor alle patiënt-pages.
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Bij opstart: header vullen en startpagina tonen.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LaadGebruikerInHeader();
        NavigeerNaarStart();
    }

    // Toont naam en profielfoto van de ingelogde patiënt in de header.
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

    // Sessie leegmaken en applicatie afsluiten.
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Gebruikersnaam = null;
        Session.ProfielData = null;
        Session.GebruikerId = 0;
        Application.Current.Shutdown();
    }
}
