using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfDokter;

// Hulpklasse om een profielfoto in een Image-control te tonen.
public static class ProfielAfbeeldingHelper
{
    // Zet de profielfoto op imgProfiel, of leeg als er geen data is.
    public static void LaadProfielAfbeelding(Image imgProfiel, byte[]? arrProfielData)
    {
        imgProfiel.Source = null;

        if (arrProfielData == null || arrProfielData.Length == 0)
        {
            return;
        }

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
