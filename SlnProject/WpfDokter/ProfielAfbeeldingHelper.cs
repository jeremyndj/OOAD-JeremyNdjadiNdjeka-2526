using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfDokter;

// Zet byte[] profielfoto uit de database om naar een BitmapImage op een WPF Image-control.
// Wordt gebruikt in MainWindow-header en op patiëntenkaarten/detail.
public static class ProfielAfbeeldingHelper
{
    public static void LaadProfielAfbeelding(Image imgProfiel, byte[]? arrProfielData)
    {
        imgProfiel.Source = null;

        if (arrProfielData == null || arrProfielData.Length == 0)
        {
            return;
        }

        // Stream moet open blijven tot EndInit; OnLoad laadt alles in geheugen en Freeze maakt thread-safe.
        BitmapImage bmpProfiel = new BitmapImage();
        using (MemoryStream stmGeheugen = new MemoryStream(arrProfielData))
        {
            bmpProfiel.BeginInit();
            bmpProfiel.CacheOption = BitmapCacheOption.OnLoad;
            bmpProfiel.StreamSource = stmGeheugen;
            bmpProfiel.EndInit();
            bmpProfiel.Freeze();
        }

        imgProfiel.Source = bmpProfiel;
    }
}
