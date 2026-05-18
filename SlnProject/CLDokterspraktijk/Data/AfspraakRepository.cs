using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Data-access voor afspraken (alle SQL voor afspraken hoort hier).
public class AfspraakRepository
{
    // Haalt alle afspraken van een dokter op voor één kalenderdag (met patiëntnaam via JOIN).
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

    // Verwijdert een afspraak (annuleren); alleen als die bij de dokter hoort.
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
