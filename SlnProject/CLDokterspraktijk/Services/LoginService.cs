using CLDokterspraktijk.Data;
using CLDokterspraktijk.Debug;
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
        string strEmailTrim = strEmail.Trim();
        Dokter? dokter = _repoDokter.HaalOpViaEmail(strEmailTrim);

        // #region agent log
        DebugAgentLog.Write(
            "LoginService.cs:Login",
            "dokter lookup",
            new
            {
                email = strEmailTrim,
                found = dokter != null,
                dokterId = dokter?.Id,
                storedHashLen = dokter?.Paswoord?.Length ?? 0
            },
            "A,C",
            "post-fix");
        // #endregion

        if (dokter == null)

        {

            return null;

        }



        // In de database staat alleen de hash; plat wachtwoord wordt hier vergeleken.
        bool bWachtwoordOk = PasswordHasher.ControleerWachtwoord(strWachtwoord, dokter.Paswoord);

        // #region agent log
        DebugAgentLog.Write(
            "LoginService.cs:Login",
            "password check",
            new { email = strEmailTrim, ok = bWachtwoordOk, inputLen = strWachtwoord?.Length ?? 0 },
            "B",
            "post-fix");
        // #endregion

        if (!bWachtwoordOk)

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
        string strEmailTrim = strEmail.Trim();
        Patient? patient = _repoPatient.HaalOpViaEmail(strEmailTrim);

        // #region agent log
        DebugAgentLog.Write(
            "LoginService.cs:LoginPatiënt",
            "patient lookup",
            new
            {
                email = strEmailTrim,
                found = patient != null,
                patientId = patient?.Id,
                storedHashLen = patient?.Paswoord?.Length ?? 0
            },
            "A,C");
        // #endregion

        if (patient == null)

        {

            return null;

        }

        bool bWachtwoordOk = PasswordHasher.ControleerWachtwoord(strWachtwoord, patient.Paswoord);

        // #region agent log
        DebugAgentLog.Write(
            "LoginService.cs:LoginPatiënt",
            "password check",
            new { email = strEmailTrim, ok = bWachtwoordOk },
            "B");
        // #endregion

        if (!bWachtwoordOk)

        {

            return null;

        }



        patient.Paswoord = string.Empty;

        return patient;

    }

}

