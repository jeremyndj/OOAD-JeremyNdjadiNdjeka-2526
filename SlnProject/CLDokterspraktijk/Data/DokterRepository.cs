using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Alle SQL voor tabel Dokter. Gebruikt door LoginService om op e-mail in te loggen.
public class DokterRepository
{
    // Zoekt één dokter op e-mail; null als het adres niet bestaat (geen exception).
    public Dokter? HaalOpViaEmail(string strEmail)
    {
        Dokter? dokter = null;

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();

            string strSql =
                "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd " +
                "FROM Dokter WHERE email = @email";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@email", strEmail);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dokter = new Dokter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Voornaam = reader.GetString(reader.GetOrdinal("voornaam")),
                            Achternaam = reader.GetString(reader.GetOrdinal("achternaam")),
                            Gsm = reader.IsDBNull(reader.GetOrdinal("gsm"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("gsm")).Trim(),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Paswoord = reader.GetString(reader.GetOrdinal("paswoord")),
                            ProfielData = reader.IsDBNull(reader.GetOrdinal("profielfotodata"))
                                ? null
                                : (byte[])reader["profielfotodata"],
                            RizivNummer = reader.GetInt32(reader.GetOrdinal("rizivnummer")).ToString(),
                            IsGeconventioneerd = reader.GetByte(reader.GetOrdinal("isgeconventioneerd")) != 0
                        };
                    }
                }
            }
        }

        return dokter;
    }
}
