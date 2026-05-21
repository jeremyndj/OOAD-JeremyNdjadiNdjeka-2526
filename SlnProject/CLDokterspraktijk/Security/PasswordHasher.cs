using System.Security.Cryptography;
using System.Text;

namespace CLDokterspraktijk.Security;

// Wachtwoordbeveiliging: SHA-256 over UTF-8, hex in kleine letters (zelfde formaat als seed-data in SQL).
public static class PasswordHasher
{
    // Gebruikt bij registratie of wachtwoord wijzigen: hash opslaan in kolom paswoord.
    public static string HashWachtwoord(string strWachtwoord)
    {
        byte[] arrBytes = SHA256.HashData(Encoding.UTF8.GetBytes(strWachtwoord));
        StringBuilder sbHash = new StringBuilder();
        foreach (byte bByte in arrBytes)
        {
            sbHash.Append(bByte.ToString("x2"));
        }

        return sbHash.ToString();
    }

    // Login: ingevoerd wachtwoord hashen en vergelijken met opgeslagen hash (geen plat wachtwoord in DB).
    public static bool ControleerWachtwoord(string strWachtwoord, string strOpgeslagenHash)
    {
        string strHashIngevoerd = HashWachtwoord(strWachtwoord);
        return string.Equals(strHashIngevoerd, strOpgeslagenHash, StringComparison.Ordinal);
    }
}
