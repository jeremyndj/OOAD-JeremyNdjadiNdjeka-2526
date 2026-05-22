using System.Net.Mail;

namespace WpfPatiënt;

// =============================================================================
// LoginValidatieHelper — formulier-validatie vóór LoginService.LoginPatiënt
// =============================================================================
// Geen SQL; fouttekst naar txtFout op LoginPage. Geen MessageBox voor validatie.
// =============================================================================
public static class LoginValidatieHelper
{
    // -------------------------------------------------------------------------
    // ValideerLoginFormulier — e-mail en wachtwoord controleren
    // -------------------------------------------------------------------------
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

    // MailAddress gooit FormatException bij ongeldige syntax; dat vangen we op als false.
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
