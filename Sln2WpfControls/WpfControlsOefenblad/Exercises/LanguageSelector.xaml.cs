using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Language Selector", Description = "ComboBox SelectionChanged event en ComboBoxItem", Order = 6, IsVisible = true)]
public partial class LanguageSelector : Page
{
    public LanguageSelector()
    {
        cbxTaal.SelectedIndex = 0;

        string[] languages = { "Nederlands", "English", "Français" };
        foreach (string language in languages)
        {
            cbxTaal.Items.Add(new ComboBoxItem() { Content = language });
        }
    }

    private void CbxTaal_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBoxItem selectedItem = (ComboBoxItem)cbxTaal.SelectedItem;

        if (selectedItem == null || selectedItem.Content.ToString() == "Kies je taal...")
        {
            return;
        }

        string? gekozenTaal = selectedItem.Content.ToString();

        switch (gekozenTaal)
        {
            case "Nederlands":
                txtBegroeting.Text = "Hallo";
                break;
            case "English":
                txtBegroeting.Text = "Hello";
                break;
            case "Français":
                txtBegroeting.Text = "Bonjour";
                break;
            default:
                txtBegroeting.Text = "...";
                break;
        }

        foreach (ComboBoxItem item in cbxTaal.Items)
        {
            item.FontWeight = FontWeights.Normal;
        }

        selectedItem.FontWeight = FontWeights.Bold;
    }
}