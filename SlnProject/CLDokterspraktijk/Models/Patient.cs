namespace CLDokterspraktijk.Models;

// =============================================================================
// Patient — domeinmodel tabel Patient
// =============================================================================
// Erft Gebruiker; geslacht als string-code (0/1/2); Notificaties als enum voor UI.
// =============================================================================
public class Patient : Gebruiker
{
    public enum Notificaties { Geen, Mail, Sms, Beide }

    public string Geslacht { get; set; }
    public string Paswoord { get; set; }
    public DateTime Geboortedatum { get; set; }
    public Notificaties NotificatieKeuze { get; set; }
}
