namespace CLDokterspraktijk.Models;



// =============================================================================

// AfspraakWeergave — afspraak + namen uit JOIN voor ListBox/kaarten in WPF

// =============================================================================

// Geen aparte databasetabel. Dokter-app: patiëntnaam via VanAfspraakEnPatient.

// Patiënt-app: dokternaam via VanAfspraakEnDokter (PatientNaam kan leeg blijven).

// =============================================================================

public class AfspraakWeergave

{

    public int Id { get; set; }

    public DateTime Moment { get; set; }

    public string Klacht { get; set; }

    public int PatientId { get; set; }

    public int DokterId { get; set; }

    public string PatientVoornaam { get; set; }

    public string PatientAchternaam { get; set; }

    public string DokterVoornaam { get; set; }

    public string DokterAchternaam { get; set; }



    public string PatientNaam

    {

        get { return PatientVoornaam + " " + PatientAchternaam; }

    }



    public string DokterNaam

    {

        get { return DokterVoornaam + " " + DokterAchternaam; }

    }



    // Maakt een weergave-object vanuit Afspraak plus namen uit Patient (dokter-agenda).

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

            PatientAchternaam = strPatientAchternaam,

            DokterVoornaam = string.Empty,

            DokterAchternaam = string.Empty

        };

    }



    // Maakt een weergave-object vanuit Afspraak plus namen uit Dokter (patiënt-overzicht).

    public static AfspraakWeergave VanAfspraakEnDokter(Afspraak afspraak, string strDokterVoornaam, string strDokterAchternaam)

    {

        return new AfspraakWeergave

        {

            Id = afspraak.Id,

            Moment = afspraak.Moment,

            Klacht = afspraak.Klacht,

            PatientId = afspraak.PatientId,

            DokterId = afspraak.DokterId,

            PatientVoornaam = string.Empty,

            PatientAchternaam = string.Empty,

            DokterVoornaam = strDokterVoornaam,

            DokterAchternaam = strDokterAchternaam

        };

    }

}

