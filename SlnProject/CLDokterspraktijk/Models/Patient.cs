namespace CLDokterspraktijk.Models;

// Patiënt met extra persoonlijke gegevens (komt overeen met tabel Patient).
public class Patient : Gebruiker
{
    public enum Notificaties { Geen, Mail, Sms, Beide }

    public string Geslacht { get; set; }
    public string Paswoord { get; set; }
    public DateTime Geboortedatum { get; set; }
    public Notificaties NotificatieKeuze { get; set; }
}
