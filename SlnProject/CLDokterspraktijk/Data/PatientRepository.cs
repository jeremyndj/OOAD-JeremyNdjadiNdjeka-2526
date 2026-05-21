using System.Data;
using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Alle SQL voor tabel Patient: lezen, toevoegen en basisgegevens wijzigen.
public class PatientRepository
{
    // Lijst voor dokter-overzicht; paswoord wordt niet opgehaald (veiligheid + niet nodig op kaart).
    public List<Patient> HaalVoorOverzicht(string strZoekterm)
    {
        List<Patient> lijst = new List<Patient>();
        string strZoek = strZoekterm == null ? string.Empty : strZoekterm.Trim();
        string strPatroon = "%" + strZoek + "%";

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT id, voornaam, achternaam, geslacht, gsm, email, geboortedatum, profielfotodata, notificaties " +
                "FROM Patient " +
                "WHERE (@zoek = N'' OR voornaam LIKE @pat OR achternaam LIKE @pat " +
                "OR (voornaam + N' ' + achternaam) LIKE @pat) " +
                "ORDER BY achternaam, voornaam";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
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
                "FROM Patient WHERE email = @email";

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
                        return null;
                    }

                    return LeesPatientUitReader(reader);
                }
            }
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

    private static Patient LeesPatientUitReader(SqlDataReader reader)
    {
        Patient patient = LeesPatientMetPaswoordUitReader(reader);
        patient.Paswoord = string.Empty;
        return patient;
    }

    // Leest patiënt inclusief paswoord-hash (alleen voor HaalOpViaEmail / login).
    private static Patient LeesPatientMetPaswoordUitReader(SqlDataReader reader)
    {
        int iGeslacht = reader.GetInt32(reader.GetOrdinal("geslacht"));
        int iNotificaties = reader.GetInt32(reader.GetOrdinal("notificaties"));
        return new Patient
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
            Paswoord = reader.GetString(reader.GetOrdinal("paswoord")),
            NotificatieKeuze = (Patient.Notificaties)iNotificaties
        };
    }
}
