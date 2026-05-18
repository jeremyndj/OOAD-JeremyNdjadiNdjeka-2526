using CLDokterspraktijk.Models;

namespace WpfDokter;

// Bewaart gegevens van de ingelogde dokter tijdens de sessie.
public static class Session
{
    // Naam rechtsboven in MainWindow.
    public static string? Gebruikersnaam { get; set; }
    // Profielfoto rechtsboven in MainWindow.
    public static byte[]? ProfielData { get; set; }
    // Id van de ingelogde dokter.
    public static int GebruikerId { get; set; }

    // Vult de sessie na een geslaagde login.
    public static void VulVanDokter(Dokter dokter)
    {
        GebruikerId = dokter.Id;
        Gebruikersnaam = dokter.Voornaam + " " + dokter.Achternaam;
        ProfielData = dokter.ProfielData;
    }

    // Leegt de sessie bij uitloggen.
    public static void Wis()
    {
        Gebruikersnaam = null;
        ProfielData = null;
        GebruikerId = 0;
    }
}
