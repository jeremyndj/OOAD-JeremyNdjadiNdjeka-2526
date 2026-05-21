using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Security;

namespace CLDokterspraktijk.Services;

// =============================================================================
// LoginService — inloggen dokter en patiënt
// =============================================================================
// Geen SQL in deze laag: orchestratie boven DokterRepository / PatientRepository en PasswordHasher.
// Bij succes wordt Paswoord op het model geleegd vóór terugkeer naar WPF (geen hash in Session).
// =============================================================================
public class LoginService
{
    private readonly DokterRepository _repoDokter = new DokterRepository();
    private readonly PatientRepository _repoPatient = new PatientRepository();

    // -------------------------------------------------------------------------
    // Login — dokter (WpfDokter); tabel Dokter, filter op e-mail
    // -------------------------------------------------------------------------
    public Dokter? Login(string strEmail, string strWachtwoord)
    {
        Dokter? dokter = _repoDokter.HaalOpViaEmail(strEmail.Trim());
        if (dokter == null)
        {
            return null;
        }

        // In de database staat alleen de hash; plat wachtwoord wordt hier vergeleken.
        if (!PasswordHasher.ControleerWachtwoord(strWachtwoord, dokter.Paswoord))
        {
            return null;
        }

        dokter.Paswoord = string.Empty;
        return dokter;
    }

    // -------------------------------------------------------------------------
    // LoginPatiënt — patiënt (WpfPatiënt); tabel Patient, filter op e-mail
    // -------------------------------------------------------------------------
    public Patient? LoginPatiënt(string strEmail, string strWachtwoord)
    {
        Patient? patient = _repoPatient.HaalOpViaEmail(strEmail.Trim());
        if (patient == null)
        {
            return null;
        }

        if (!PasswordHasher.ControleerWachtwoord(strWachtwoord, patient.Paswoord))
        {
            return null;
        }

        patient.Paswoord = string.Empty;
        return patient;
    }
}
