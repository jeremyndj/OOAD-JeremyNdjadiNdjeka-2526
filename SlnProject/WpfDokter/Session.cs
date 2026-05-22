using CLDokterspraktijk.Models;

namespace WpfDokter;

// =============================================================================
// Session — ingelogde dokter in geheugen
// =============================================================================
// Geen database; VulVanDokter na login, Wis bij uitloggen. GebruikerId 0 = niet ingelogd.
// =============================================================================
public static class Session
{
    // Volledige naam in de header (voornaam + achternaam).
    public static string? Gebruikersnaam { get; set; }
    // Binaire profielfoto voor imgProfiel in MainWindow.
    public static byte[]? ProfielData { get; set; }
    // Primary key van Dokter; 0 betekent niet ingelogd.
    public static int GebruikerId { get; set; }

    // Na geslaagde LoginService.Login: velden vullen vanuit het Dokter-object (zonder paswoord-hash).
    public static void VulVanDokter(Dokter dokter)
    {
        GebruikerId = dokter.Id;
        Gebruikersnaam = dokter.Voornaam + " " + dokter.Achternaam;
        ProfielData = dokter.ProfielData;
    }

    // Bij uitloggen terug naar anonieme staat zodat Window_Loaded-logica opnieuw login toont.
    public static void Wis()
    {
        Gebruikersnaam = null;
        ProfielData = null;
        GebruikerId = 0;
    }
}
