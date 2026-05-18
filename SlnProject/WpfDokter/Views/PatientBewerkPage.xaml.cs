using System.Windows;
using System.Windows.Controls;

namespace WpfDokter.Views;

// Patiëntgegevens wijzigen (formulier volgt later).
public partial class PatientBewerkPage : Page
{
    private readonly int _iPatientId;

    public PatientBewerkPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
        txtTitel.Text = "Patiënt aanpassen (id " + iPatientId + ")";
    }

    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new PatientenPage());
    }
}
