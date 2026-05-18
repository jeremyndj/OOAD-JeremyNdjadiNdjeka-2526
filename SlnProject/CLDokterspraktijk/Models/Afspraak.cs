namespace CLDokterspraktijk.Models;

// Strikte 1-op-1 mapping met tabel [dbo].[Afspraak]: id, moment, klacht, patient_id, dokter_id.
// Gebruik AfspraakWeergave voor lijsten met patiëntnaam (komt uit JOIN met Patient).
public class Afspraak
{
    public int Id { get; set; }
    public DateTime Moment { get; set; }
    public string Klacht { get; set; }
    public int PatientId { get; set; }
    public int DokterId { get; set; }
}
