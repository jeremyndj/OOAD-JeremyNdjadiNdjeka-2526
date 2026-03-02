using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Chatbox", Description = "multiline TextBox, TextBlock opmaak", Order = 3, IsVisible = true)]
public partial class ChatBox : Page
{
    public ChatBox()
    {
        InitializeComponent();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        txtChat.Inlines.Add(new Run ($"{inpName.Text}: ") { FontWeight = FontWeights.Bold });
        txtChat.Inlines.Add(inpMessage.Text);
        inpName.Clear();
        inpMessage.Clear();
    }
}
