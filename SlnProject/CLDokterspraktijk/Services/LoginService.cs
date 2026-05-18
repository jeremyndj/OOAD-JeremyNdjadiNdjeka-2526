using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Security;

namespace CLDokterspraktijk.Services;

// Inloggen van een dokter via e-mail en wachtwoord.
public class LoginService
{
    private readonly DokterRepository _repoDokter = new DokterRepository();

    // Geeft de dokter terug bij succes, anders null (onbekend e-mail of fout wachtwoord).
    public Dokter? Login(string strEmail, string strWachtwoord)
    {
        Dokter? dokter = _repoDokter.HaalOpViaEmail(strEmail.Trim());
        if (dokter == null)
        {
            return null;
        }

        // Wachtwoord hashen en vergelijken met kolom paswoord in de database.
        if (!PasswordHasher.ControleerWachtwoord(strWachtwoord, dokter.Paswoord))
        {
            return null;
        }

        // Hash niet doorgeven naar WPF.
        dokter.Paswoord = string.Empty;
        return dokter;
    }
}
