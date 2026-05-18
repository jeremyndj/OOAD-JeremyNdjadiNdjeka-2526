using System.Security.Cryptography;
using System.Text;

namespace CLDokterspraktijk.Security;

// Hasht wachtwoorden met SHA-256 (zelfde formaat als in de database).
public static class PasswordHasher
{
    // Maakt een onleesbare hex-string van het wachtwoord.
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

    // Hasht het ingevulde wachtwoord en vergelijkt met de hash uit de database.
    public static bool ControleerWachtwoord(string strWachtwoord, string strOpgeslagenHash)
    {
        string strHashIngevoerd = HashWachtwoord(strWachtwoord);
        return string.Equals(strHashIngevoerd, strOpgeslagenHash, StringComparison.Ordinal);
    }
}
