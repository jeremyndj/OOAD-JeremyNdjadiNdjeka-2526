using System.Windows.Controls;

namespace WpfDokter.Views;

// =============================================================================
// ProfielPage — profiel van de ingelogde dokter (placeholder)
// =============================================================================
// Navigatie: MainWindow (zijmenu of profielknop in header) → deze Page.
// XAML bevat nog geen interactieve velden; er is geen code-behind-logica buiten InitializeComponent.
// Later: gegevens uit Session of DokterRepository, eventueel bewerken van eigen profiel.
// =============================================================================
public partial class ProfielPage : Page
{
    public ProfielPage()
    {
        InitializeComponent();
    }
}
