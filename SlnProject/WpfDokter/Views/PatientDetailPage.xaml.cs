using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// Toont basisinformatie van één patiënt (uit de database).
public partial class PatientDetailPage : Page
{
    private readonly int _iPatientId;
    private readonly PatientService _svcPatient = new PatientService();

    public PatientDetailPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Patient? patient = _svcPatient.HaalOpId(_iPatientId);
            if (patient == null)
            {
                txtTitel.Text = "Patiënt niet gevonden";
                txtNaam.Text = "Er is geen patiënt met dit id.";
                txtEmail.Text = string.Empty;
                txtGsm.Text = string.Empty;
                txtGeboortedatum.Text = string.Empty;
                txtGeslacht.Text = string.Empty;
                txtNotificaties.Text = string.Empty;
                return;
            }

            txtTitel.Text = "Patiënt — " + patient.Voornaam + " " + patient.Achternaam;
            txtNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            txtEmail.Text = patient.Email;
            txtGsm.Text = string.IsNullOrEmpty(patient.Gsm) ? "—" : patient.Gsm;
            txtGeboortedatum.Text = patient.Geboortedatum.ToString("d MMMM yyyy", new CultureInfo("nl-BE"));
            txtGeslacht.Text = VertaalGeslacht(patient.Geslacht);
            txtNotificaties.Text = patient.NotificatieKeuze.ToString();

            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, patient.ProfielData);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Gegevens laden is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // Codes uit de seed-database: 0 = vrouw, 1 = man, 2 = anders (conventie praktijk).
    private static string VertaalGeslacht(string strCode)
    {
        return strCode switch
        {
            "0" => "Vrouw",
            "1" => "Man",
            "2" => "Anders",
            _ => "Code " + strCode
        };
    }

    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new PatientenPage());
    }
}
