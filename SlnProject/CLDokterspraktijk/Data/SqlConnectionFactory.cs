using System.Configuration;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// Opent SQL-verbindingen met de connection string uit config.
public static class SqlConnectionFactory
{
    // Leest connStr uit App.config en maakt een nieuwe SqlConnection.
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
