namespace CLDokterspraktijk.Models;

// Model voor de UI: combineert afspraakgegevens met patiëntnaam uit een SQL-JOIN.
// Geen aparte tabel in de database; alleen een C#-klasse om ListBox-regels te vullen.
public class AfspraakWeergave
{
    public int Id { get; set; }
    public DateTime Moment { get; set; }
    public string Klacht { get; set; }
    public int PatientId { get; set; }
    public int DokterId { get; set; }
    public string PatientVoornaam { get; set; }
    public string PatientAchternaam { get; set; }

    public string PatientNaam
    {
        get { return PatientVoornaam + " " + PatientAchternaam; }
    }

    // Maakt een weergave-object vanuit de strikte Afspraak-entiteit plus namen uit Patient.
    public static AfspraakWeergave VanAfspraakEnPatient(Afspraak afspraak, string strPatientVoornaam, string strPatientAchternaam)
    {
        return new AfspraakWeergave
        {
            Id = afspraak.Id,
            Moment = afspraak.Moment,
            Klacht = afspraak.Klacht,
            PatientId = afspraak.PatientId,
            DokterId = afspraak.DokterId,
            PatientVoornaam = strPatientVoornaam,
            PatientAchternaam = strPatientAchternaam
        };
    }
}
