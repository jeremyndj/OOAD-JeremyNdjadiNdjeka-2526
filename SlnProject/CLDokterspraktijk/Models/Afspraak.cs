namespace CLDokterspraktijk.Models;

// =============================================================================
// Afspraak — domeinmodel tabel Afspraak
// =============================================================================
// 1-op-1 met kolommen id, moment, klacht, patient_id, dokter_id.
// Voor lijsten met namen: AfspraakWeergave (JOIN in repository).
// =============================================================================
public class Afspraak
{
    public int Id { get; set; }
    public DateTime Moment { get; set; }
    public string Klacht { get; set; }
    public int PatientId { get; set; }
    public int DokterId { get; set; }
}
