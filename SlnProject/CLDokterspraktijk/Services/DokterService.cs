using CLDokterspraktijk.Data;
using CLDokterspraktijk.Models;

namespace CLDokterspraktijk.Services;

// =============================================================================
// DokterService — dokterslijst voor patiënt-UI (geen login hier)
// =============================================================================
// WpfPatiënt gebruikt HaalVoorKeuzelijst op Afspraak maken; geen SQL in WPF.
// =============================================================================
public class DokterService
{
    private readonly DokterRepository _repoDokter = new DokterRepository();

    // -------------------------------------------------------------------------
    // HaalVoorKeuzelijst — alle dokters, gesorteerd op naam
    // -------------------------------------------------------------------------
    public List<Dokter> HaalVoorKeuzelijst()
    {
        return _repoDokter.HaalVoorKeuzelijst();
    }

    // Volledige doktergegevens voor contactkaart na keuze in ComboBox.
    public Dokter? HaalOpId(int iDokterId)
    {
        return _repoDokter.HaalOpId(iDokterId);
    }
}
