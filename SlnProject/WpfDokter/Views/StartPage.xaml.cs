using System.Windows;
using System.Windows.Controls;

namespace WpfDokter.Views;

// Startpagina: uitleg over de app en snelkoppelingen naar andere schermen.
public partial class StartPage : Page
{
    public StartPage()
    {
        InitializeComponent();
    }

    // Persoonlijke welkomsttekst op basis van de sessie.
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        string naam = string.IsNullOrEmpty(Session.Gebruikersnaam) ? "dokter" : Session.Gebruikersnaam;
        txtWelkom.Text = $"Welkom, {naam}";
    }

    private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new AfsprakenPage());
    }

    private void BtnPatienten_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new PatientenPage());
    }
}
