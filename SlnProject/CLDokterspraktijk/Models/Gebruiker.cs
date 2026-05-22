namespace CLDokterspraktijk.Models;

// =============================================================================
// Gebruiker — abstracte basis voor Dokter en Patient
// =============================================================================
// Gemeenschappelijke kolommen uit de database; geen string.Empty property-initializers.
// =============================================================================
public abstract class Gebruiker
{
    public int Id { get; set; }
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }
    public string Gsm { get; set; }
    public string Email { get; set; }
    public byte[]? ProfielData { get; set; }
}
