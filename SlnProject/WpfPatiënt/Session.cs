namespace WpfPatiënt;

// Bewaart gegevens van de ingelogde patiënt tijdens de sessie.
public static class Session
{
    public static string? Gebruikersnaam { get; set; }
    public static byte[]? ProfielData { get; set; }
    public static int GebruikerId { get; set; }
}
