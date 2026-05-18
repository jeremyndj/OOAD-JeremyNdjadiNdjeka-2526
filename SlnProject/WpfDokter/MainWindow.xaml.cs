using System.Windows;
using System.Windows.Controls;
using WpfDokter.Views;

namespace WpfDokter;

// Hoofdvenster: zijmenu, header en Frame voor alle dokter-pages (login start in het Frame).
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

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

    // Na geslaagde login vanuit LoginPage: knoppen activeren en patiëntenoverzicht openen.
    public void NaLogin()
    {
        ZetMenuIngelogd(true);
        LaadGebruikerInHeader();
        NavigeerNaarPatienten();
    }

    // Zijmenu- en headerknoppen: uit tot login geslaagd (knoppen blijven zichtbaar).
    private void ZetMenuIngelogd(bool bIngelogd)
    {
        btnStart.IsEnabled = bIngelogd;
        btnAfspraken.IsEnabled = bIngelogd;
        btnPatienten.IsEnabled = bIngelogd;
        btnLogout.IsEnabled = bIngelogd;
        btnProfiel.IsEnabled = bIngelogd;
    }

    // Header zonder ingelogde gebruiker (tijdens login).
    private void WisGebruikerInHeader()
    {
        txtGebruikersnaam.Text = "Niet ingelogd";
        ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, null);
    }

    // Toont naam en profielfoto uit Session (gevuld bij login).
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

    private void NavigeerNaarPatienten()
    {
        fraMain.Navigate(new PatientenPage());
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

    private void BtnPatienten_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarPatienten();
    }

    private void BtnProfiel_Click(object sender, RoutedEventArgs e)
    {
        NavigeerNaarProfiel();
    }

    // Sessie leeg, menu uitschakelen en loginpagina in het Frame tonen.
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Wis();
        ZetMenuIngelogd(false);
        WisGebruikerInHeader();
        fraMain.Navigate(new LoginPage());
    }
}
