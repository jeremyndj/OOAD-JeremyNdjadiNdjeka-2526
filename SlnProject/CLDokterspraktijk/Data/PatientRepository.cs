using System.Data;
using CLDokterspraktijk.Debug;
using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// =============================================================================
// PatientRepository — SQL voor tabel Patient
// =============================================================================
// SELECT/INSERT/UPDATE/DELETE met parameters; exceptions doorgeven naar WPF (try-catch in .xaml.cs).
// Geen UI-meldingen; geen platte wachtwoorden in SELECT voor overzichtskaarten.
// =============================================================================
public class PatientRepository
{
    // -------------------------------------------------------------------------
    // HaalVoorOverzicht — patiënten van één dokter (via Afspraak), met zoekfilter op naam
    // -------------------------------------------------------------------------
    // Geen SELECT DISTINCT op profielfotodata (image-kolom is niet vergelijkbaar in SQL Server).
    // Unieke patiënt-ids komen uit een subquery; daarna één rij per patiënt uit Patient.
    // Paswoord wordt niet opgehaald (veiligheid + niet nodig op de contactkaart).
    // -------------------------------------------------------------------------
    public List<Patient> HaalVoorOverzicht(int iDokterId, string strZoekterm)
    {
        List<Patient> lijst = new List<Patient>();
        string strZoek = strZoekterm == null ? string.Empty : strZoekterm.Trim();
        string strPatroon = "%" + strZoek + "%";

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT p.id, p.voornaam, p.achternaam, p.geslacht, p.gsm, p.email, p.geboortedatum, p.profielfotodata, p.notificaties " +
                "FROM Patient p " +
                "WHERE p.id IN (SELECT DISTINCT a.patient_id FROM Afspraak a WHERE a.dokter_id = @dokterId) " +
                "AND (@zoek = N'' OR p.voornaam LIKE @pat OR p.achternaam LIKE @pat " +
                "OR (p.voornaam + N' ' + p.achternaam) LIKE @pat) " +
                "ORDER BY p.achternaam, p.voornaam";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@dokterId", iDokterId);
                cmd.Parameters.AddWithValue("@zoek", strZoek);
                cmd.Parameters.AddWithValue("@pat", strPatroon);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lijst.Add(LeesPatientUitReader(reader));
                    }
                }
            }
        }

        return lijst;
    }

    // Zoekt één patiënt op e-mail voor login; null als het adres niet bestaat (geen exception).
    public Patient? HaalOpViaEmail(string strEmail)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties " +
                "FROM Patient WHERE LTRIM(RTRIM(email)) = LTRIM(RTRIM(@email))";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@email", strEmail);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return LeesPatientMetPaswoordUitReader(reader);
                }
            }
        }
    }

    // Eén patiënt op id; null als id niet bestaat.
    public Patient? HaalOpId(int iPatientId)
    {
        // #region agent log
        DebugAgentLog.Write(
            "PatientRepository.cs:HaalOpId",
            "entry",
            new { patientId = iPatientId },
            "E");
        // #endregion

        try
        {
            using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
            {
                conn.Open();
                string strSql =
                    "SELECT id, voornaam, achternaam, geslacht, gsm, email, geboortedatum, profielfotodata, notificaties " +
                    "FROM Patient WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", iPatientId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // #region agent log
                            DebugAgentLog.Write(
                                "PatientRepository.cs:HaalOpId",
                                "no row",
                                new { patientId = iPatientId },
                                "E");
                            // #endregion
                            return null;
                        }

                        Patient patient = LeesPatientUitReader(reader);
                        // #region agent log
                        DebugAgentLog.Write(
                            "PatientRepository.cs:HaalOpId",
                            "success",
                            new { patientId = iPatientId, naam = patient.Voornaam + " " + patient.Achternaam },
                            "E");
                        // #endregion
                        return patient;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // #region agent log
            DebugAgentLog.Write(
                "PatientRepository.cs:HaalOpId",
                "exception",
                new { patientId = iPatientId, type = ex.GetType().Name, message = ex.Message },
                "E");
            // #endregion
            throw;
        }
    }

    // Nieuwe patiënt; strEmail en strPaswoordHash zijn verplichte kolommen in de tabel.
    // Geeft het nieuwe id terug na INSERT.
    public int VoegToe(string strVoornaam, string strAchternaam, int iGeslacht, DateTime datumGeboorte,
        string strEmail, string strPaswoordHash, byte[]? arrProfielData)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "INSERT INTO Patient (voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties) " +
                "VALUES (@voornaam, @achternaam, @geslacht, NULL, @email, @paswoord, @geboortedatum, @profielfoto, 0); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@voornaam", strVoornaam.Trim());
                cmd.Parameters.AddWithValue("@achternaam", strAchternaam.Trim());
                cmd.Parameters.AddWithValue("@geslacht", iGeslacht);
                cmd.Parameters.AddWithValue("@email", strEmail);
                cmd.Parameters.AddWithValue("@paswoord", strPaswoordHash);
                cmd.Parameters.AddWithValue("@geboortedatum", datumGeboorte.Date);
                VoegProfielParameterToe(cmd, arrProfielData);

                object? resultaat = cmd.ExecuteScalar();
                if (resultaat == null)
                {
                    return 0;
                }

                return Convert.ToInt32(resultaat);
            }
        }
    }

    // Wijzigt profielgegevens van een patiënt (dokter- of patiëntformulier).
    public bool WerkBij(int iPatientId, string strVoornaam, string strAchternaam, int iGeslacht, DateTime datumGeboorte,
        string strGsm, int iNotificaties, byte[]? arrProfielData)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "UPDATE Patient SET voornaam = @voornaam, achternaam = @achternaam, geslacht = @geslacht, geboortedatum = @geboortedatum, " +
                "gsm = @gsm, notificaties = @notificaties, profielfotodata = @profielfoto WHERE id = @id";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", iPatientId);
                cmd.Parameters.AddWithValue("@voornaam", strVoornaam.Trim());
                cmd.Parameters.AddWithValue("@achternaam", strAchternaam.Trim());
                cmd.Parameters.AddWithValue("@geslacht", iGeslacht);
                cmd.Parameters.AddWithValue("@geboortedatum", datumGeboorte.Date);
                if (string.IsNullOrWhiteSpace(strGsm))
                {
                    cmd.Parameters.AddWithValue("@gsm", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@gsm", strGsm.Trim());
                }

                cmd.Parameters.AddWithValue("@notificaties", iNotificaties);
                VoegProfielParameterToe(cmd, arrProfielData);

                int iRijen = cmd.ExecuteNonQuery();
                return iRijen > 0;
            }
        }
    }

    // Verwijdert één patiënt op id; roep eerst AfspraakRepository.VerwijderAlleVanPatient aan.
    public bool Verwijder(int iPatientId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql = "DELETE FROM Patient WHERE id = @id";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", iPatientId);
                int iRijen = cmd.ExecuteNonQuery();
                return iRijen > 0;
            }
        }
    }

    // Controleert of e-mailadres al door een andere patiënt wordt gebruikt.
    public bool BestaatEmail(string strEmail, int iUitgeslotenPatientId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql = "SELECT COUNT(1) FROM Patient WHERE email = @email AND id <> @id";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@email", strEmail);
                cmd.Parameters.AddWithValue("@id", iUitgeslotenPatientId);

                int iAantal = Convert.ToInt32(cmd.ExecuteScalar());
                return iAantal > 0;
            }
        }
    }

    private static void VoegProfielParameterToe(SqlCommand cmd, byte[]? arrProfielData)
    {
        if (arrProfielData == null || arrProfielData.Length == 0)
        {
            cmd.Parameters.Add("@profielfoto", SqlDbType.Image).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@profielfoto", SqlDbType.Image).Value = arrProfielData;
        }
    }

    // Leest patiënt zonder paswoord-kolom (overzicht, profiel, detail).
    private static Patient LeesPatientUitReader(SqlDataReader reader)
    {
        return LeesPatientBasisUitReader(reader);
    }

    // Leest patiënt inclusief paswoord-hash (alleen HaalOpViaEmail / login; SELECT moet paswoord bevatten).
    private static Patient LeesPatientMetPaswoordUitReader(SqlDataReader reader)
    {
        Patient patient = LeesPatientBasisUitReader(reader);
        patient.Paswoord = reader.GetString(reader.GetOrdinal("paswoord")).Trim();
        return patient;
    }

    // Gemeenschappelijke velden; geen paswoord (kolom staat niet in elke SELECT).
    private static Patient LeesPatientBasisUitReader(SqlDataReader reader)
    {
        int iGeslacht = reader.GetInt32(reader.GetOrdinal("geslacht"));
        int iNotificaties = reader.GetInt32(reader.GetOrdinal("notificaties"));
        Patient patient = new Patient
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Voornaam = reader.GetString(reader.GetOrdinal("voornaam")),
            Achternaam = reader.GetString(reader.GetOrdinal("achternaam")),
            Geslacht = iGeslacht.ToString(),
            Gsm = reader.IsDBNull(reader.GetOrdinal("gsm"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("gsm")).Trim(),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Geboortedatum = reader.GetDateTime(reader.GetOrdinal("geboortedatum")),
            ProfielData = reader.IsDBNull(reader.GetOrdinal("profielfotodata"))
                ? null
                : (byte[])reader["profielfotodata"],
            Paswoord = string.Empty,
            NotificatieKeuze = (Patient.Notificaties)iNotificaties
        };
        return patient;
    }
}
