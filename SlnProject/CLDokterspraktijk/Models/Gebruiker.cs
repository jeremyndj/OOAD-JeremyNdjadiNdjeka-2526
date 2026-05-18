namespace CLDokterspraktijk.Models;

// Basisgegevens die dokter en patiënt gemeen hebben.
public abstract class Gebruiker
{
    public int Id { get; set; }
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }
    public string Gsm { get; set; }
    public string Email { get; set; }
    public byte[]? ProfielData { get; set; }
}
