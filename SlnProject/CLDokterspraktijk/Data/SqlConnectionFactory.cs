using System.Configuration;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Centrale plek voor databaseverbindingen: leest connStr uit App.config van het startproject (WpfDokter).
// Elke repository opent hier een SqlConnection; geen connection string hardcoded in SQL-klassen.
public static class SqlConnectionFactory
{
    public static SqlConnection MaakVerbinding()
    {
        string? strConnection = ConfigurationManager.ConnectionStrings["connStr"]?.ConnectionString;
        if (string.IsNullOrWhiteSpace(strConnection))
        {
            throw new InvalidOperationException("Connection string 'connStr' ontbreekt in App.config.");
        }

        return new SqlConnection(strConnection);
    }
}
