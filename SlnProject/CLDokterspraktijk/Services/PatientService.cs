using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Security;

namespace CLDokterspraktijk.Services;

// =============================================================================
// PatientService — orchestratie patiënt-CRUD
// =============================================================================
// WpfDokter en WpfPatiënt roepen deze service aan; SQL blijft in PatientRepository.
// Startwachtwoord bij nieuwe patiënt via PasswordHasher.
// =============================================================================
public class PatientService
{
    private readonly PatientRepository _repoPatient = new PatientRepository();

    // Patiënten van één dokter (via tabel Afspraak); iDokterId komt uit Session.GebruikerId in WpfDokter.
    public List<Patient> HaalVoorOverzicht(int iDokterId, string strZoekterm)
    {
        return _repoPatient.HaalVoorOverzicht(iDokterId, strZoekterm);
    }

    public Patient? HaalOpId(int iPatientId)
    {
        return _repoPatient.HaalOpId(iPatientId);
    }

    // Voegt patiënt toe; e-mail en startwachtwoord worden hier afgeleid (niet op het dokterformulier).
    public int VoegToe(string strVoornaam, string strAchternaam, int iGeslacht, DateTime datumGeboorte, byte[]? arrProfielData)
    {
        string strEmail = MaakUniekEmail(strVoornaam, strAchternaam, 0);
        string strPaswoordHash = PasswordHasher.HashWachtwoord(MaakStartWachtwoord(strVoornaam, strAchternaam, datumGeboorte));
        return _repoPatient.VoegToe(strVoornaam, strAchternaam, iGeslacht, datumGeboorte, strEmail, strPaswoordHash, arrProfielData);
    }

    public bool WerkBij(int iPatientId, string strVoornaam, string strAchternaam, int iGeslacht, DateTime datumGeboorte,
        string strGsm, int iNotificaties, byte[]? arrProfielData)
    {
        return _repoPatient.WerkBij(iPatientId, strVoornaam, strAchternaam, iGeslacht, datumGeboorte, strGsm, iNotificaties, arrProfielData);
    }

    // Verwijdert eerst alle afspraken van de patiënt, daarna de patiënt zelf (foreign key Afspraak → Patient).
    public bool VerwijderInclusiefAfspraken(int iPatientId)
    {
        AfspraakRepository repoAfspraak = new AfspraakRepository();
        repoAfspraak.VerwijderAlleVanPatient(iPatientId);
        return _repoPatient.Verwijder(iPatientId);
    }

    // E-mail voor nieuwe patiënt: voornaam.achternaam@dokterspraktijk.local; bij conflict teller achtervoegen.
    private string MaakUniekEmail(string strVoornaam, string strAchternaam, int iUitgeslotenId)
    {
        string strBasis = NormaliseerVoorEmail(strVoornaam) + "." + NormaliseerVoorEmail(strAchternaam) + "@dokterspraktijk.local";
        string strEmail = strBasis;
        int iTeller = 1;

        while (_repoPatient.BestaatEmail(strEmail, iUitgeslotenId))
        {
            strEmail = NormaliseerVoorEmail(strVoornaam) + "." + NormaliseerVoorEmail(strAchternaam) + iTeller.ToString() + "@dokterspraktijk.local";
            iTeller = iTeller + 1;
        }

        return strEmail;
    }

    private static string NormaliseerVoorEmail(string strTekst)
    {
        string str = strTekst.Trim().ToLowerInvariant();
        str = str.Replace(" ", string.Empty);
        return str;
    }

    // Startwachtwoord afgeleid van gegevens (geen vast wachtwoord in broncode); patiënt kan later wijzigen.
    private static string MaakStartWachtwoord(string strVoornaam, string strAchternaam, DateTime datumGeboorte)
    {
        return NormaliseerVoorEmail(strVoornaam) + NormaliseerVoorEmail(strAchternaam) + datumGeboorte.ToString("yyyyMMdd");
    }
}
