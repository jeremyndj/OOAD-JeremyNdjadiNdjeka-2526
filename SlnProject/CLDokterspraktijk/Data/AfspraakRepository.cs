using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// SQL voor tabel Afspraak, inclusief JOIN met Patient voor namen op het scherm.
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
