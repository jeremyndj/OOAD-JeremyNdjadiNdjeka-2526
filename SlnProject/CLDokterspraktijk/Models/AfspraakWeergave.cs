namespace CLDokterspraktijk.Models;

// Read model: afspraak + patiëntnaam uit JOIN (geen aparte tabel in de database).
public class AfspraakWeergave
{
    public int Id { get; set; }
    public DateTime Moment { get; set; }
    public string Klacht { get; set; }
    public int PatientId { get; set; }
    public int DokterId { get; set; }
    public string PatientVoornaam { get; set; }
    public string PatientAchternaam { get; set; }

    public string PatientNaam => PatientVoornaam + " " + PatientAchternaam;

    // Bouwt weergave op basis van strikte Afspraak-entiteit + namen uit Patient.
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
