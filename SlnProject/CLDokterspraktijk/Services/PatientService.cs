using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;

namespace CLDokterspraktijk.Services;

// Patiëntenlijst voor het dokter-overzicht.
public class PatientService
{
    private readonly PatientRepository _repoPatient = new PatientRepository();

    public List<Patient> HaalVoorOverzicht(string strZoekterm)
    {
        return _repoPatient.HaalVoorOverzicht(strZoekterm);
    }

    public Patient? HaalOpId(int iPatientId)
    {
        return _repoPatient.HaalOpId(iPatientId);
    }
}
