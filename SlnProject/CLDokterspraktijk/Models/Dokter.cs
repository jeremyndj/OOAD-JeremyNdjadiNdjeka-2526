namespace CLDokterspraktijk.Models;

// Dokter met praktijkgegevens.
public class Dokter : Gebruiker
{
    // Gehasht wachtwoord uit de database (alleen gebruikt bij login).
    public string Paswoord { get; set; }
    public string RizivNummer { get; set; }
    public bool IsGeconventioneerd { get; set; }
}
