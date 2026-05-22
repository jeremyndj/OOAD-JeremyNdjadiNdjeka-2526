namespace CLDokterspraktijk.Models;

// =============================================================================
// Dokter — domeinmodel tabel Dokter
// =============================================================================
// Erft Gebruiker; Paswoord bevat hash (na login leeg gemaakt in LoginService).
// =============================================================================
public class Dokter : Gebruiker
{
    // Gehasht wachtwoord uit de database (alleen gebruikt bij login).
    public string Paswoord { get; set; }
    public string RizivNummer { get; set; }
    public bool IsGeconventioneerd { get; set; }
}
