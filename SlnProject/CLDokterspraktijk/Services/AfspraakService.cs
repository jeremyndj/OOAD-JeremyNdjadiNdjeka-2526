using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;

namespace CLDokterspraktijk.Services;

// Afspraken ophalen en annuleren voor de ingelogde dokter.
public class AfspraakService
{
    private readonly AfspraakRepository _repoAfspraak = new AfspraakRepository();

    // Lijst voor de UI: AfspraakWeergave (JOIN met Patient), niet de kale entiteit alleen.
    public List<AfspraakWeergave> HaalAfsprakenOpDag(int iDokterId, DateTime datumDag)
    {
        return _repoAfspraak.HaalOpDag(iDokterId, datumDag);
    }

    // Annuleert een afspraak (verwijdert uit de database).
    public bool Annuleer(int iAfspraakId, int iDokterId)
    {
        return _repoAfspraak.Verwijder(iAfspraakId, iDokterId);
    }
}
