using System.Configuration;
using CLDokterspraktijk.Debug;
using Microsoft.Data.SqlClient;

namespace CLDokterspraktijk.Data;

// =============================================================================
// SqlConnectionFactory — databaseverbindingen
// =============================================================================
// Leest connection string connStr uit App.config; elke repository gebruikt MaakVerbinding().
// Geen SQL-queries in deze klasse; geen WPF-afhankelijkheid.
// =============================================================================
public static class SqlConnectionFactory
{
    public static SqlConnection MaakVerbinding()
    {
        string? strConnection = ConfigurationManager.ConnectionStrings["connStr"]?.ConnectionString;
        if (string.IsNullOrWhiteSpace(strConnection))
        {
            throw new InvalidOperationException("Connection string 'connStr' ontbreekt in App.config.");
        }

        // #region agent log
        DebugAgentLog.Write(
            "SqlConnectionFactory.cs:MaakVerbinding",
            "connection string resolved",
            new { initialCatalog = ParseInitialCatalog(strConnection) },
            "A");
        // #endregion

        return new SqlConnection(strConnection);
    }

    private static string? ParseInitialCatalog(string strConnection)
    {
        foreach (string part in strConnection.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = part.Trim();
            if (trimmed.StartsWith("Initial Catalog=", StringComparison.OrdinalIgnoreCase)
                || trimmed.StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed[(trimmed.IndexOf('=') + 1)..].Trim();
            }
        }

        return null;
    }
}
