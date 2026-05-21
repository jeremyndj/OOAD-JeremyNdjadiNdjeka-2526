using System.Windows;

namespace WpfDokter;

// =============================================================================
// App — entry point van de dokter-WPF-applicatie
// =============================================================================
// StartupUri in App.xaml wijst naar MainWindow.xaml (eerste venster dat opent).
// Geen globale event handlers hier: login, sessie en navigatie zitten in MainWindow en Pages.
// Connection string staat in App.config (connStr) en wordt gelezen door CLDokterspraktijk.
// =============================================================================
public partial class App : Application
{
}
