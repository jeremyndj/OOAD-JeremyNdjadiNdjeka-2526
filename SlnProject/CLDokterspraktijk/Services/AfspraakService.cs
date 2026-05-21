using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;

namespace CLDokterspraktijk.Services;

// Afspraken voor de ingelogde dokter: ophalen per dag en annuleren via repository.
public class AfspraakService
{
    private readonly AfspraakRepository _repoAfspraak = new AfspraakRepository();

    // Retourneert AfspraakWeergave (afspraak + patiëntnaam uit JOIN), niet alleen entiteit Afspraak.
    public List<AfspraakWeergave> HaalAfsprakenOpDag(int iDokterId, DateTime datumDag)
    {
        return _repoAfspraak.HaalOpDag(iDokterId, datumDag);
    }

    public bool Annuleer(int iAfspraakId, int iDokterId)
    {
        return _repoAfspraak.Verwijder(iAfspraakId, iDokterId);
    }
}
