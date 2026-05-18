using System.Net.Mail;

namespace WpfDokter;

// Controleert het loginformulier voordat er wordt ingelogd.
public static class LoginValidatieHelper
{
    // Geeft een foutmelding terug, of null als alles geldig is.
    public static string? ValideerLoginFormulier(string strEmail, string strWachtwoord)
    {
        if (string.IsNullOrWhiteSpace(strEmail))
        {
            return "Vul een e-mailadres in.";
        }

        if (!IsGeldigEmailadres(strEmail.Trim()))
        {
            return "Vul een geldig e-mailadres in.";
        }

        if (string.IsNullOrWhiteSpace(strWachtwoord))
        {
            return "Vul een wachtwoord in.";
        }

        return null;
    }

    // Controleert of de tekst een geldig e-mailformaat heeft.
    private static bool IsGeldigEmailadres(string strEmail)
    {
        try
        {
            MailAddress adres = new MailAddress(strEmail);
            return adres.Address.Equals(strEmail, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
