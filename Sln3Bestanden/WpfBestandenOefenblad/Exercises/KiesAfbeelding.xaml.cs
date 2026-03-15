using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Kies afbeelding", Description = "OpenFileDialog om een jpg/jpeg te kiezen en in een Image te tonen", Order = 3, IsVisible = true)]
public partial class KiesAfbeelding : Page
{
    public KiesAfbeelding()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog dialog = new OpenFolderDialog();
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string chosenFolderName;
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult == true)
        {
            // user picked a folder and pressed OK
            chosenFolderName = dialog.FolderName;
            txtBestandnaam.Text = "Gekozen map: " + chosenFolderName;
        }
        else
        {
            // user cancelled or escaped dialog window
            txtBestandnaam.Text = "Kiezen gestopt";
        }
    }
}
