using System.Windows;
using System.Windows.Controls;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad.Exercises;

[NavPage(title: "MessageBox", description: "Dialoogvensters", order: 10)]
public partial class MessageBoxDialog : Page
{
    public MessageBoxDialog()
    {
        InitializeComponent();
        MessageBox.Show("Wil je de gegevens opslaan?", "Bevestigen", MessageBoxButton.YesNo);
        MessageBoxResult result = MessageBox.Show("De gegevens zijn opgeslagen.");
    }
}
