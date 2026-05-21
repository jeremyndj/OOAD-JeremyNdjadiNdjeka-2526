using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Security;

namespace CLDokterspraktijk.Services;

// Inloggen van een dokter: e-mail opzoeken in de database, wachtwoord hashen en vergelijken.
// Geen SQL in deze laag — alleen orchestratie boven DokterRepository en PasswordHasher.
public class LoginService
{
    private readonly DokterRepository _repoDokter = new DokterRepository();

    // Bij succes een Dokter zonder paswoord-hash (veilig voor WPF); bij fout null.
    public Dokter? Login(string strEmail, string strWachtwoord)
    {
        Dokter? dokter = _repoDokter.HaalOpViaEmail(strEmail.Trim());
        if (dokter == null)
        {
            return null;
        }

        // In de database staat alleen de hash; plat wachtwoord wordt hier gehasht en vergeleken.
        if (!PasswordHasher.ControleerWachtwoord(strWachtwoord, dokter.Paswoord))
        {
            return null;
        }

        dokter.Paswoord = string.Empty;
        return dokter;
    }
}
