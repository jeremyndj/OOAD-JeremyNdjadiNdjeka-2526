using System.Windows;
using System.Windows.Controls;

namespace WpfDokter.Views;

// Patiënt verwijderen (bevestiging en delete volgen later).
public partial class PatientVerwijderPage : Page
{
    private readonly int _iPatientId;

    public PatientVerwijderPage(int iPatientId)
    {
        _iPatientId = iPatientId;
        InitializeComponent();
        txtTitel.Text = "Patiënt verwijderen (id " + iPatientId + ")";
    }

    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new PatientenPage());
    }
}
