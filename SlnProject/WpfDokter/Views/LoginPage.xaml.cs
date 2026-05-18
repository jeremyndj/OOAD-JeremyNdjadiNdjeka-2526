using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;
using WpfDokter;

namespace WpfDokter.Views;

// Login binnen het Frame van MainWindow.
public partial class LoginPage : Page
{
    private readonly LoginService _svcLogin = new LoginService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private void BtnInloggen_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        string strEmail = txtEmail.Text;
        string strWachtwoord = pwdWachtwoord.Password;

        string? strValidatieFout = LoginValidatieHelper.ValideerLoginFormulier(strEmail, strWachtwoord);
        if (strValidatieFout != null)
        {
            ToonFout(strValidatieFout);
            return;
        }

        try
        {
            Dokter? dokter = _svcLogin.Login(strEmail, strWachtwoord);
            if (dokter == null)
            {
                ToonFout("E-mailadres of wachtwoord is niet correct.");
                return;
            }

            Session.VulVanDokter(dokter);

            if (Window.GetWindow(this) is MainWindow vensterHoofd)
            {
                vensterHoofd.NaLogin();
            }
        }
        catch (Exception ex)
        {
            ToonFout("Inloggen is mislukt: " + ex.Message);
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
