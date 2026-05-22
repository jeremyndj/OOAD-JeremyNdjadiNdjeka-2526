namespace WpfDokter;

// =============================================================================
// PatientFormulierValidatieHelper — formulier-validatie patiënt CRUD
// =============================================================================
// PatientBewerkPage: melding in txtFout; daarna PatientService voor INSERT/UPDATE.
// =============================================================================
public static class PatientFormulierValidatieHelper
{
    // -------------------------------------------------------------------------
    // Valideer — voornaam, achternaam, geboortedatum, geslacht
    // -------------------------------------------------------------------------
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
