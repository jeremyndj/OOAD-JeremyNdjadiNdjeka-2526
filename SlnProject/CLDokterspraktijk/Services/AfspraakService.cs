using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;

namespace CLDokterspraktijk.Services;

// =============================================================================
// AfspraakService — afspraken voor dokter en patiënt
// =============================================================================
// Validatie voor nieuwe afspraak (datum, klacht, dokter) gebeurt hier, niet in WPF.
// WpfDokter: HaalAfsprakenOpDag, Annuleer. WpfPatiënt: HaalAfsprakenVoorPatiënt, MaakAfspraak, AnnuleerDoorPatiënt.
// =============================================================================
public class AfspraakService
{
    private readonly AfspraakRepository _repoAfspraak = new AfspraakRepository();

    public List<AfspraakWeergave> HaalAfsprakenOpDag(int iDokterId, DateTime datumDag)
    {
        return _repoAfspraak.HaalOpDag(iDokterId, datumDag);
    }

    public bool Annuleer(int iAfspraakId, int iDokterId)
    {
        return _repoAfspraak.Verwijder(iAfspraakId, iDokterId);
    }

    public List<AfspraakWeergave> HaalAfsprakenVoorPatiënt(int iPatientId)
    {
        return _repoAfspraak.HaalVoorPatient(iPatientId);
    }

    // Patiënt annuleert eigen toekomstige afspraak (DELETE met patient_id in WHERE).
    public bool AnnuleerDoorPatiënt(int iAfspraakId, int iPatientId)
    {
        return _repoAfspraak.VerwijderDoorPatient(iAfspraakId, iPatientId);
    }

    public int MaakAfspraak(int iPatientId, int iDokterId, DateTime moment, string strKlacht)
    {
        if (iPatientId <= 0)
        {
            return 0;
        }

        if (iDokterId <= 0)
        {
            return 0;
        }

        if (string.IsNullOrWhiteSpace(strKlacht))
        {
            return 0;
        }

        if (moment <= DateTime.Now)
        {
            return 0;
        }

        return _repoAfspraak.VoegToe(iPatientId, iDokterId, moment, strKlacht);
    }
}
