using CLDokterspraktijk.Models;

namespace WpfPatiënt;

// =============================================================================
// Session — statische sessie voor de ingelogde patiënt (één app-run, geen database)
// =============================================================================
// MainWindow en pages lezen GebruikerId (= Patient.id) en weergavenaam in de header.
// Na login: VulVanPatient; bij uitloggen: Wis zodat Window_Loaded opnieuw LoginPage toont.
// =============================================================================
public static class Session
{
    // Volledige naam in de header (voornaam + achternaam).
    public static string? Gebruikersnaam { get; set; }
    // Binaire profielfoto voor imgProfiel in MainWindow.
    public static byte[]? ProfielData { get; set; }
    // Primary key van Patient; 0 betekent niet ingelogd.
    public static int GebruikerId { get; set; }

    // -------------------------------------------------------------------------
    // VulVanPatient — na geslaagde LoginService.LoginPatiënt
    // -------------------------------------------------------------------------
    public static void VulVanPatient(Patient patient)
    {
        GebruikerId = patient.Id;
        Gebruikersnaam = patient.Voornaam + " " + patient.Achternaam;
        ProfielData = patient.ProfielData;
    }

    // -------------------------------------------------------------------------
    // Wis — bij uitloggen; zelfde staat als vóór eerste login
    // -------------------------------------------------------------------------
    public static void Wis()
    {
        Gebruikersnaam = null;
        ProfielData = null;
        GebruikerId = 0;
    }
}
