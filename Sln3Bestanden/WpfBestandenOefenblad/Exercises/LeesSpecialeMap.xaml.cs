using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees speciale map", Description = "Drie knoppen: Desktop, Documenten, Afbeeldingen; bestanden met grootte en aanmaakdatum in TextBlock", Order = 6, IsVisible = true)]
public partial class LeesSpecialeMap : Page
{
    public LeesSpecialeMap()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = System.IO.Path.Combine(folderPath, "myfile.txt");

        string[] bestanden = Directory.GetFiles(filePath);

        foreach (string bestand in bestanden) 
        {
            FileInfo info = new FileInfo(bestand);
            txtBestanden.Text += $"{info.Name} | {info.Length} bytes | {info.LastWriteTime}\n";
        }
    }
}
