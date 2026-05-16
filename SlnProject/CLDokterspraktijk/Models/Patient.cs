namespace CLDokterspraktijk.Models;

public class Patient : Gebruiker
{
    public string Geslacht { get; set; }
    public string Paswoord { get; set; }
    public DateTime Geboortedatum { get; set; }
    public enum Notificaties { Geen, Mail, Sms, Beide }
}
