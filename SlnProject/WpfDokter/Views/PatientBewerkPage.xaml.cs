using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Services;
using Microsoft.Win32;

namespace WpfDokter.Views;

// =============================================================================
// PatientBewerkPage — toevoegen (id=0) en wijzigen (id>0) op één formulier
// =============================================================================
// Verplichte velden: voornaam, familienaam, geslacht (radio 0/1/2), geboortedatum.
// Optioneel: profielfoto (bytes in _arrProfielData, kolom profielfotodata in SQL).
// Formchecking:
// - Geen MessageBox voor validatie; fouten in txtFout.
// - Bij NIEUWE patiënt: btnOpslaan disabled tot IsFormulierCompleet true (live bij elk veld).
// - Bij WIJZIGEN: Opslaan altijd enabled; validatie bij klik op Opslaan.
// E-mail/wachtwoord bij insert worden in PatientService afgeleid (niet op dit formulier).
// =============================================================================
public partial class PatientBewerkPage : Page
{
    // 0 = INSERT; anders primary key van bestaande patiënt voor UPDATE.
    private readonly int _iPatientId;
    private readonly PatientService _svcPatient = new PatientService();
    // Geselecteerde of geladen foto; null of lege array = geen afbeelding in database.
    private byte[]? _arrProfielData;

    public PatientBewerkPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — modus bepalen (nieuw vs wijzigen) en formulier vullen
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_iPatientId == 0)
        {
            txtTitel.Text = "Nieuwe patiënt toevoegen";
            WerkProfielWeergaveBij();
            WerkOpslaanKnopStatus();
            return;
        }

        txtTitel.Text = "Patiënt wijzigen";
        btnOpslaan.IsEnabled = true;

        try
        {
            CLDokterspraktijk.Models.Patient? patient = _svcPatient.HaalOpId(_iPatientId);
            if (patient == null)
            {
                ToonFout("De patiënt kon niet worden geladen.");
                btnOpslaan.IsEnabled = false;
                return;
            }

            txtVoornaam.Text = patient.Voornaam;
            txtAchternaam.Text = patient.Achternaam;
            dpGeboortedatum.SelectedDate = patient.Geboortedatum;
            ZetGeslachtRadioknop(patient.Geslacht);
            _arrProfielData = patient.ProfielData;
            WerkProfielWeergaveBij();
        }
        catch (Exception ex)
        {
            ToonFout("Laden is mislukt: " + ex.Message);
            btnOpslaan.IsEnabled = false;
        }
    }

    // -------------------------------------------------------------------------
    // ZetGeslachtRadioknop — databasecode string naar juiste RadioButton
    // -------------------------------------------------------------------------
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

    // -------------------------------------------------------------------------
    // HaalGekozenGeslacht — UI → int voor SQL-kolom geslacht
    // -------------------------------------------------------------------------
    // Retourneert -1 als geen radioknop geselecteerd is (validatie faalt dan).
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

    // -------------------------------------------------------------------------
    // WerkOpslaanKnopStatus — alleen voor modus "nieuwe patiënt"
    // -------------------------------------------------------------------------
    // Houdt Opslaan uit tot alle verplichte velden geldig zijn; profielfoto telt niet mee.
    private void WerkOpslaanKnopStatus()
    {
        if (_iPatientId != 0)
        {
            return;
        }

        int iGeslacht = HaalGekozenGeslacht();
        bool bGeslachtGekozen = iGeslacht >= 0;
        bool bCompleet = PatientFormulierValidatieHelper.IsFormulierCompleet(
            txtVoornaam.Text,
            txtAchternaam.Text,
            dpGeboortedatum.SelectedDate,
            bGeslachtGekozen);

        btnOpslaan.IsEnabled = bCompleet;
    }

    // Gekoppeld aan TextChanged (tekstvelden) en Checked (radioknoppen) in XAML.
    private void FormulierVeld_Gewijzigd(object sender, RoutedEventArgs e)
    {
        WerkOpslaanKnopStatus();
    }

    private void DpGeboortedatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        WerkOpslaanKnopStatus();
    }

    // -------------------------------------------------------------------------
    // BtnKiesProfiel_Click — optionele foto via OpenFileDialog
    // -------------------------------------------------------------------------
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

    // Verwijdert de gekozen foto uit geheugen; bij opslaan wordt NULL in profielfotodata gezet.
    private void BtnVerwijderProfiel_Click(object sender, RoutedEventArgs e)
    {
        _arrProfielData = null;
        WerkProfielWeergaveBij();
    }

    // -------------------------------------------------------------------------
    // WerkProfielWeergaveBij — preview: foto of placeholder-icoon
    // -------------------------------------------------------------------------
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
            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, _arrProfielData);
        }
    }

    // -------------------------------------------------------------------------
    // BtnOpslaan_Click — validatie + INSERT of UPDATE + terug naar overzicht
    // -------------------------------------------------------------------------
    private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        string strVoornaam = txtVoornaam.Text;
        string strAchternaam = txtAchternaam.Text;
        DateTime? datumGeboorte = dpGeboortedatum.SelectedDate;
        int iGeslacht = HaalGekozenGeslacht();
        bool bGeslachtGekozen = iGeslacht >= 0;

        string? strValidatieFout = PatientFormulierValidatieHelper.Valideer(
            strVoornaam, strAchternaam, datumGeboorte, bGeslachtGekozen);
        if (strValidatieFout != null)
        {
            ToonFout(strValidatieFout);
            return;
        }

        DateTime datum = datumGeboorte!.Value;

        try
        {
            if (_iPatientId == 0)
            {
                int iNieuwId = _svcPatient.VoegToe(strVoornaam, strAchternaam, iGeslacht, datum, _arrProfielData);
                if (iNieuwId <= 0)
                {
                    ToonFout("De patiënt kon niet worden toegevoegd.");
                    return;
                }
            }
            else
            {
                bool bGelukt = _svcPatient.WerkBij(_iPatientId, strVoornaam, strAchternaam, iGeslacht, datum, _arrProfielData);
                if (!bGelukt)
                {
                    ToonFout("De wijzigingen konden niet worden opgeslagen.");
                    return;
                }
            }

            if (NavigationService != null)
            {
                NavigationService.Navigate(new PatientenPage());
            }
        }
        catch (Exception ex)
        {
            ToonFout("Opslaan is mislukt: " + ex.Message);
        }
    }

    // Annuleren: geen database-actie; direct terug naar PatientenPage.
    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientenPage());
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
