using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// =============================================================================
// AfspraakRepository — SQL voor tabel Afspraak
// =============================================================================
// SELECT met JOIN voor weergave; INSERT; DELETE bij annuleren. Geen UI in deze laag.
// =============================================================================
public class AfspraakRepository
{
    // Alle afspraken van één dokter tussen middernacht en middernacht+1 dag.
    public List<AfspraakWeergave> HaalOpDag(int iDokterId, DateTime datumDag)
    {
        List<AfspraakWeergave> lijstAfspraken = new List<AfspraakWeergave>();
        DateTime startDag = datumDag.Date;
        DateTime eindeDag = startDag.AddDays(1);

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id, " +
                "p.voornaam, p.achternaam " +
                "FROM Afspraak a " +
                "INNER JOIN Patient p ON p.id = a.patient_id " +
                "WHERE a.dokter_id = @dokterId " +
                "AND a.moment >= @startDag AND a.moment < @eindeDag " +
                "ORDER BY a.moment";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@dokterId", iDokterId);
                cmd.Parameters.AddWithValue("@startDag", startDag);
                cmd.Parameters.AddWithValue("@eindeDag", eindeDag);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraakKern = new Afspraak
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Moment = reader.GetDateTime(reader.GetOrdinal("moment")),
                            Klacht = reader.GetValue(reader.GetOrdinal("klacht"))?.ToString() ?? string.Empty,
                            PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                            DokterId = reader.GetInt32(reader.GetOrdinal("dokter_id"))
                        };
                        string strVoornaam = reader.GetString(reader.GetOrdinal("voornaam"));
                        string strAchternaam = reader.GetString(reader.GetOrdinal("achternaam"));
                        AfspraakWeergave weergave = AfspraakWeergave.VanAfspraakEnPatient(afspraakKern, strVoornaam, strAchternaam);
                        lijstAfspraken.Add(weergave);
                    }
                }
            }
        }

        return lijstAfspraken;
    }

    // Alle afspraken van één patiënt, met dokternaam uit JOIN (WpfPatiënt — Mijn afspraken).
    public List<AfspraakWeergave> HaalVoorPatient(int iPatientId)
    {
        List<AfspraakWeergave> lijstAfspraken = new List<AfspraakWeergave>();

        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id, " +
                "d.voornaam, d.achternaam " +
                "FROM Afspraak a " +
                "INNER JOIN Dokter d ON d.id = a.dokter_id " +
                "WHERE a.patient_id = @patientId " +
                "ORDER BY a.moment";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@patientId", iPatientId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraakKern = new Afspraak
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Moment = reader.GetDateTime(reader.GetOrdinal("moment")),
                            Klacht = reader.GetValue(reader.GetOrdinal("klacht"))?.ToString() ?? string.Empty,
                            PatientId = reader.GetInt32(reader.GetOrdinal("patient_id")),
                            DokterId = reader.GetInt32(reader.GetOrdinal("dokter_id"))
                        };
                        string strDokterVoornaam = reader.GetString(reader.GetOrdinal("voornaam"));
                        string strDokterAchternaam = reader.GetString(reader.GetOrdinal("achternaam"));
                        AfspraakWeergave weergave = AfspraakWeergave.VanAfspraakEnDokter(afspraakKern, strDokterVoornaam, strDokterAchternaam);
                        lijstAfspraken.Add(weergave);
                    }
                }
            }
        }

        return lijstAfspraken;
    }

    // Nieuwe afspraak voor patiënt; geeft nieuw id terug na INSERT (0 bij fout op database).
    public int VoegToe(int iPatientId, int iDokterId, DateTime moment, string strKlacht)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql =
                "INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id) " +
                "VALUES (@moment, @klacht, @patientId, @dokterId); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@moment", moment);
                cmd.Parameters.AddWithValue("@klacht", strKlacht.Trim());
                cmd.Parameters.AddWithValue("@patientId", iPatientId);
                cmd.Parameters.AddWithValue("@dokterId", iDokterId);

                object? resultaat = cmd.ExecuteScalar();
                if (resultaat == null)
                {
                    return 0;
                }

                return Convert.ToInt32(resultaat);
            }
        }
    }

    // Verwijdert alle afspraken van één patiënt (vóór DELETE op Patient wegens foreign key).
    public int VerwijderAlleVanPatient(int iPatientId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql = "DELETE FROM Afspraak WHERE patient_id = @patientId";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@patientId", iPatientId);
                return cmd.ExecuteNonQuery();
            }
        }
    }

    // Annuleren door patiënt = DELETE; alleen eigen afspraken (patient_id in WHERE).
    public bool VerwijderDoorPatient(int iAfspraakId, int iPatientId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql = "DELETE FROM Afspraak WHERE id = @id AND patient_id = @patientId";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", iAfspraakId);
                cmd.Parameters.AddWithValue("@patientId", iPatientId);
                int iRijen = cmd.ExecuteNonQuery();
                return iRijen > 0;
            }
        }
    }

    // Annuleren = DELETE; alleen rijen van deze dokter (dokter_id in WHERE).
    public bool Verwijder(int iAfspraakId, int iDokterId)
    {
        using (SqlConnection conn = SqlConnectionFactory.MaakVerbinding())
        {
            conn.Open();
            string strSql = "DELETE FROM Afspraak WHERE id = @id AND dokter_id = @dokterId";

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", iAfspraakId);
                cmd.Parameters.AddWithValue("@dokterId", iDokterId);
                int iRijen = cmd.ExecuteNonQuery();
                return iRijen > 0;
            }
        }
    }
}
