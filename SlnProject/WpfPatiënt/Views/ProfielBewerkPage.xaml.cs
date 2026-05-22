using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;
using Microsoft.Win32;

namespace WpfPatiënt.Views;

// =============================================================================
// ProfielBewerkPage — eigen profiel wijzigen (ingelogde patiënt)
// =============================================================================
// UPDATE via PatientService.WerkBij inclusief gsm en notificaties (enum als radiobuttons).
// Na opslaan: Session + header MainWindow bijwerken, terug naar ProfielPage.
// =============================================================================
public partial class ProfielBewerkPage : Page
{
    private readonly PatientService _svcPatient = new PatientService();
    private byte[]? _arrProfielData;

    public ProfielBewerkPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (Session.GebruikerId <= 0)
        {
            ToonFout("U bent niet ingelogd.");
            btnOpslaan.IsEnabled = false;
            return;
        }

        try
        {
            Patient? patient = _svcPatient.HaalOpId(Session.GebruikerId);
            if (patient == null)
            {
                ToonFout("Uw profiel kon niet worden geladen.");
                btnOpslaan.IsEnabled = false;
                return;
            }

            txtVoornaam.Text = patient.Voornaam;
            txtAchternaam.Text = patient.Achternaam;
            txtGsm.Text = patient.Gsm;
            dpGeboortedatum.SelectedDate = patient.Geboortedatum;
            txtEmailReadonly.Text = patient.Email;
            ZetGeslachtRadioknop(patient.Geslacht);
            ZetNotificatieRadioknop(patient.NotificatieKeuze);
            _arrProfielData = patient.ProfielData;
            WerkProfielWeergaveBij();
        }
        catch (Exception ex)
        {
            ToonFout("Laden is mislukt: " + ex.Message);
            btnOpslaan.IsEnabled = false;
        }
    }

    private void ZetGeslachtRadioknop(string strGeslachtCode)
    {
        rbVrouw.IsChecked = false;
        rbMan.IsChecked = false;
        rbAnders.IsChecked = false;

        if (strGeslachtCode == "0")
        {
            rbVrouw.IsChecked = true;
        }
        else if (strGeslachtCode == "1")
        {
            rbMan.IsChecked = true;
        }
        else if (strGeslachtCode == "2")
        {
            rbAnders.IsChecked = true;
        }
    }

    private int HaalGekozenGeslacht()
    {
        if (rbVrouw.IsChecked == true)
        {
            return 0;
        }

        if (rbMan.IsChecked == true)
        {
            return 1;
        }

        if (rbAnders.IsChecked == true)
        {
            return 2;
        }

        return -1;
    }

    private void ZetNotificatieRadioknop(Patient.Notificaties notificatie)
    {
        rbNotificatieGeen.IsChecked = false;
        rbNotificatieMail.IsChecked = false;
        rbNotificatieSms.IsChecked = false;
        rbNotificatieBeide.IsChecked = false;

        if (notificatie == Patient.Notificaties.Geen)
        {
            rbNotificatieGeen.IsChecked = true;
        }
        else if (notificatie == Patient.Notificaties.Mail)
        {
            rbNotificatieMail.IsChecked = true;
        }
        else if (notificatie == Patient.Notificaties.Sms)
        {
            rbNotificatieSms.IsChecked = true;
        }
        else if (notificatie == Patient.Notificaties.Beide)
        {
            rbNotificatieBeide.IsChecked = true;
        }
    }

    private int HaalGekozenNotificatie()
    {
        if (rbNotificatieGeen.IsChecked == true)
        {
            return 0;
        }

        if (rbNotificatieMail.IsChecked == true)
        {
            return 1;
        }

        if (rbNotificatieSms.IsChecked == true)
        {
            return 2;
        }

        if (rbNotificatieBeide.IsChecked == true)
        {
            return 3;
        }

        return -1;
    }

    private void BtnKiesProfiel_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialoog = new OpenFileDialog
        {
            Title = "Profielfoto kiezen",
            Filter = "Afbeeldingen (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
        };

        bool? bGekozen = dialoog.ShowDialog();
        if (bGekozen != true || string.IsNullOrEmpty(dialoog.FileName))
        {
            return;
        }

        try
        {
            byte[] arrBytes = File.ReadAllBytes(dialoog.FileName);
            if (arrBytes.Length > 5 * 1024 * 1024)
            {
                ToonFout("De afbeelding is te groot (max. 5 MB).");
                return;
            }

            _arrProfielData = arrBytes;
            VerbergFout();
            WerkProfielWeergaveBij();
        }
        catch (Exception ex)
        {
            ToonFout("Foto laden is mislukt: " + ex.Message);
        }
    }

    private void BtnVerwijderProfiel_Click(object sender, RoutedEventArgs e)
    {
        _arrProfielData = null;
        WerkProfielWeergaveBij();
    }

    private void WerkProfielWeergaveBij()
    {
        if (_arrProfielData == null || _arrProfielData.Length == 0)
        {
            imgProfiel.Source = null;
            txtProfielPlaceholder.Visibility = Visibility.Visible;
        }
        else
        {
            txtProfielPlaceholder.Visibility = Visibility.Collapsed;
            try
            {
                ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, _arrProfielData);
            }
            catch (Exception ex)
            {
                imgProfiel.Source = null;
                txtProfielPlaceholder.Visibility = Visibility.Visible;
                ToonFout("Profielfoto tonen is mislukt: " + ex.Message);
            }
        }
    }

    private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        string strVoornaam = txtVoornaam.Text;
        string strAchternaam = txtAchternaam.Text;
        string strGsm = txtGsm.Text;
        DateTime? datumGeboorte = dpGeboortedatum.SelectedDate;
        int iGeslacht = HaalGekozenGeslacht();
        bool bGeslachtGekozen = iGeslacht >= 0;
        int iNotificatie = HaalGekozenNotificatie();
        bool bNotificatieGekozen = iNotificatie >= 0;

        string? strValidatieFout = ProfielFormulierValidatieHelper.Valideer(
            strVoornaam, strAchternaam, datumGeboorte, bGeslachtGekozen, strGsm, bNotificatieGekozen);
        if (strValidatieFout != null)
        {
            ToonFout(strValidatieFout);
            return;
        }

        DateTime datum = datumGeboorte!.Value;

        try
        {
            bool bGelukt = _svcPatient.WerkBij(Session.GebruikerId, strVoornaam, strAchternaam, iGeslacht, datum,
                strGsm, iNotificatie, _arrProfielData);
            if (!bGelukt)
            {
                ToonFout("De wijzigingen konden niet worden opgeslagen.");
                return;
            }

            Patient? patient = _svcPatient.HaalOpId(Session.GebruikerId);
            if (patient != null)
            {
                Session.VulVanPatient(patient);
                Window? venster = Window.GetWindow(this);
                MainWindow? vensterHoofd = venster as MainWindow;
                if (vensterHoofd != null)
                {
                    vensterHoofd.LaadGebruikerInHeader();
                }
            }

            if (NavigationService != null)
            {
                NavigationService.Navigate(new ProfielPage());
            }
        }
        catch (Exception ex)
        {
            ToonFout("Opslaan is mislukt: " + ex.Message);
        }
    }

    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new ProfielPage());
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
