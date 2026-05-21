using System.Windows.Controls;

namespace WpfPatiënt.Views;

// Startpagina voor de patiënt na inloggen (inhoud volgt in latere fase).
public partial class StartPage : Page
{
    // Alleen XAML laden; nog geen Loaded-handler of navigatie in code-behind.
    public StartPage()
    {
        InitializeComponent();
    }
}
