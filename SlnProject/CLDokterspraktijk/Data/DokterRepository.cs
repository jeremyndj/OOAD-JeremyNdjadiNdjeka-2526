using CLDokterspraktijk.Debug;
using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// =============================================================================
// DokterRepository — SQL voor tabel Dokter
// =============================================================================
// Login op e-mail; lijst voor patiënt-app (AfspraakMakenPage). Exceptions doorgeven naar WPF.
// =============================================================================
public class DokterRepository
{
    // Zoekt één dokter op e-mail; null als het adres niet bestaat (geen exception).
    public Dokter? HaalOpViaEmail(string strEmail)
    {
        Dokter? dokter = null;

        try
        {
            using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
            {
                conn.Open();

                string strSql =
                    "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd " +
                    "FROM Dokter WHERE LTRIM(RTRIM(email)) = LTRIM(RTRIM(@email))";

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
                                Email = reader.GetString(reader.GetOrdinal("email")).Trim(),
                                Paswoord = reader.GetString(reader.GetOrdinal("paswoord")).Trim(),
                                ProfielData = LeesProfielBytes(reader, reader.GetOrdinal("profielfotodata")),
                                RizivNummer = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("rizivnummer"))).ToString(),
                                IsGeconventioneerd = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("isgeconventioneerd"))) != 0
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // #region agent log
            DebugAgentLog.Write(
                "DokterRepository.cs:HaalOpViaEmail",
                "exception",
                new { email = strEmail, type = ex.GetType().Name, message = ex.Message },
                "C",
                "post-fix");
            // #endregion
            throw;
        }

        return dokter;
    }

    // Veilig profielfoto-bytes uitlezen (image-kolom kan als byte[] of DBNull binnenkomen).
    private static byte[]? LeesProfielBytes(SqlDataReader reader, int iOrdinal)
    {
        if (reader.IsDBNull(iOrdinal))
        {
            return null;
        }

        object objWaarde = reader.GetValue(iOrdinal);
        if (objWaarde is byte[] arrData)
        {
            return arrData;
        }

        return null;
    }

    // Eén dokter op id voor contactkaart op Afspraak maken; null als id niet bestaat.
    public Dokter? HaalOpId(int iDokterId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT id, voornaam, achternaam, gsm, email, profielfotodata, rizivnummer, isgeconventioneerd " +
                "FROM Dokter WHERE id = @id";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", iDokterId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return LeesDokterZonderPaswoordUitReader(reader);
                }
            }
        }
    }

    // Lijst dokters voor ComboBox op Afspraak maken (id + naam; geen paswoord).
    public List<Dokter> HaalVoorKeuzelijst()
    {
        List<Dokter> lijstDokters = new List<Dokter>();

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT id, voornaam, achternaam FROM Dokter ORDER BY achternaam, voornaam";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dokter dokter = new Dokter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Voornaam = reader.GetString(reader.GetOrdinal("voornaam")),
                            Achternaam = reader.GetString(reader.GetOrdinal("achternaam")),
                            Gsm = string.Empty,
                            Email = string.Empty,
                            Paswoord = string.Empty,
                            RizivNummer = string.Empty,
                            IsGeconventioneerd = false
                        };
                        lijstDokters.Add(dokter);
                    }
                }
            }
        }

        return lijstDokters;
    }

    // Leest dokter voor UI; paswoord-kolom wordt niet gebruikt (lege string op model).
    private static Dokter LeesDokterZonderPaswoordUitReader(SqlDataReader reader)
    {
        return new Dokter
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Voornaam = reader.GetString(reader.GetOrdinal("voornaam")),
            Achternaam = reader.GetString(reader.GetOrdinal("achternaam")),
            Gsm = reader.IsDBNull(reader.GetOrdinal("gsm"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("gsm")).Trim(),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Paswoord = string.Empty,
            ProfielData = reader.IsDBNull(reader.GetOrdinal("profielfotodata"))
                ? null
                : (byte[])reader["profielfotodata"],
            RizivNummer = reader.GetInt32(reader.GetOrdinal("rizivnummer")).ToString(),
            IsGeconventioneerd = reader.GetByte(reader.GetOrdinal("isgeconventioneerd")) != 0
        };
    }
}
