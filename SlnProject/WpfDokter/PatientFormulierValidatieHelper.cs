namespace WpfDokter;

// Validatie van het patiëntformulier (toevoegen/wijzigen) vóór opslaan in de database.
public static class PatientFormulierValidatieHelper
{
    // Controleert voornaam, familienaam en geboortedatum; retourneert fouttekst of null.
    public static string? Valideer(string strVoornaam, string strAchternaam, DateTime? datumGeboorte, bool bGeslachtGekozen)
    {
        if (string.IsNullOrWhiteSpace(strVoornaam))
        {
            return "Vul een voornaam in.";
        }

        if (string.IsNullOrWhiteSpace(strAchternaam))
        {
            return "Vul een familienaam in.";
        }

        if (!bGeslachtGekozen)
        {
            return "Kies een geslacht.";
        }

        if (datumGeboorte == null)
        {
            return "Kies een geboortedatum.";
        }

        if (datumGeboorte.Value.Date > DateTime.Today)
        {
            return "De geboortedatum mag niet in de toekomst liggen.";
        }

        return null;
    }

    // True als alle verplichte velden ingevuld zijn (profielfoto telt niet mee).
    public static bool IsFormulierCompleet(string strVoornaam, string strAchternaam, DateTime? datumGeboorte, bool bGeslachtGekozen)
    {
        return Valideer(strVoornaam, strAchternaam, datumGeboorte, bGeslachtGekozen) == null;
    }
}
