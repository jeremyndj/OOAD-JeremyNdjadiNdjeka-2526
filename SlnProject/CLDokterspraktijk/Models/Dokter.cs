namespace CLDokterspraktijk.Models;

public class Dokter : Gebruiker
{
    public string RizivNummer { get; set; }
    public bool IsGeconventioneerd { get; set; }
}
