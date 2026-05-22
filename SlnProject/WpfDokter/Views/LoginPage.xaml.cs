using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Debug;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;
using WpfDokter;

namespace WpfDokter.Views;

// =============================================================================
// LoginPage — inloggen dokter binnen MainWindow.fraMain
// =============================================================================
// Scheiding van verantwoordelijkheden:
// - Deze Page: invoer lezen, formulier valideren, fout tonen in txtFout (geen MessageBox voor validatie)
// - LoginValidatieHelper: regels op e-mail/wachtwoord vóór database-aanroep
// - LoginService (CLDokterspraktijk): SQL op tabel Dokter, hash-vergelijking
// - Session: ingelogde dokter onthouden voor andere pages (o.a. AfsprakenPage, header)
// =============================================================================
public partial class LoginPage : Page
{
    // Eén service-instantie per Page; bevat geen UI-state.
    private readonly LoginService _svcLogin = new LoginService();

    public LoginPage()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // BtnInloggen_Click — hoofdflow login
    // -------------------------------------------------------------------------
    private void BtnInloggen_Click(object sender, RoutedEventArgs e)
    {
        // Stap 1: vorige foutmelding wissen zodat de gebruiker een schone poging ziet.
        VerbergFout();

        // Stap 2: waarden uit de WPF-controls halen (geen data binding).
        string strEmail = txtEmail.Text;
        string strWachtwoord = pwdWachtwoord.Password;

        // Stap 3: client-side validatie; bij fout stoppen en txtFout tonen.
        string? strValidatieFout = LoginValidatieHelper.ValideerLoginFormulier(strEmail, strWachtwoord);
        if (strValidatieFout != null)
        {
            ToonFout(strValidatieFout);
            return;
        }

        // Stap 4: database-login via class library; exception = connection string / SQL-server.
        try
        {
            // #region agent log
            DebugAgentLog.Write(
                "LoginPage.xaml.cs:BtnInloggen_Click",
                "login poging",
                new { email = strEmail.Trim(), wachtwoordLen = strWachtwoord.Length },
                "B",
                "post-fix");
            // #endregion

            Dokter? dokter = _svcLogin.Login(strEmail, strWachtwoord);
            if (dokter == null)
            {
                // Bewust één melding: geen onderscheid tussen onbekend e-mail en fout wachtwoord.
                ToonFout("E-mailadres of wachtwoord is niet correct.");
                return;
            }

            // Stap 5: sessie vullen (GebruikerId nodig voor filter op dokter_id bij afspraken).
            Session.VulVanDokter(dokter);

            // #region agent log
            DebugAgentLog.Write(
                "LoginPage.xaml.cs:BtnInloggen_Click",
                "login succes",
                new { dokter.Id, dokter.Email },
                "B",
                "post-fix");
            // #endregion

            // Stap 6: parent MainWindow activeren (menu, header, navigatie naar patiënten).
            Window? venster = Window.GetWindow(this);
            MainWindow? vensterHoofd = venster as MainWindow;
            if (vensterHoofd != null)
            {
                vensterHoofd.NaLogin();
            }
        }
        catch (Exception ex)
        {
            ToonFout("Inloggen is mislukt: " + ex.Message);
        }
    }

    // Toont een fouttekst in het rode TextBlock onder het formulier (Visibility.Visible).
    private void ToonFout(string strMelding)
    {
        txtFout.Text = strMelding;
        txtFout.Visibility = Visibility.Visible;
    }

    // Verbergt txtFout vóór een nieuwe validatie- of loginpoging.
    private void VerbergFout()
    {
        txtFout.Visibility = Visibility.Collapsed;
        txtFout.Text = string.Empty;
    }
}
