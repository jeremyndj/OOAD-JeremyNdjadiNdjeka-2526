using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfDokter;

// =============================================================================
// ProfielAfbeeldingHelper — byte[] naar WPF Image
// =============================================================================
// Aanroep in .xaml.cs binnen try-catch (AGENTS.md); kan exception gooien bij ongeldige bytes.
// =============================================================================
public static class ProfielAfbeeldingHelper
{
    // -------------------------------------------------------------------------
    // LaadProfielAfbeelding — BitmapImage op imgProfiel zetten
    // -------------------------------------------------------------------------
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
