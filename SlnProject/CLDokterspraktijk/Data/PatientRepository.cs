using CLDokterspraktijk.Models;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Data-access voor patiënten (alle SQL voor patiënten hoort hier).
public class PatientRepository
{
    // Lijst voor overzicht: filter op naam; paswoord niet uit de database (leeg op object).
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
                        lijst.Add(patient);
                    }
                }
            }
        }

        return lijst;
    }

    // Eén patiënt op id (voor detailpagina); geen paswoord in de query.
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
                        Paswoord = string.Empty,
                        NotificatieKeuze = (Patient.Notificaties)iNotificaties
                    };
                }
            }
        }
    }
}
