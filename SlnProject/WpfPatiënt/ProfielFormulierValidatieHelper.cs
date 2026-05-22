namespace WpfPatiënt;

// =============================================================================
// ProfielFormulierValidatieHelper — formulier-validatie profiel bewerken
// =============================================================================
// ProfielBewerkPage: txtFout; daarna PatientService.WerkBij voor UPDATE.
// =============================================================================
public static class ProfielFormulierValidatieHelper
{
    // -------------------------------------------------------------------------
    // Valideer — naam, geboortedatum, geslacht, gsm, notificatie
    // -------------------------------------------------------------------------
    public static string? Valideer(string strVoornaam, string strAchternaam, DateTime? datumGeboorte, bool bGeslachtGekozen,
        string strGsm, bool bNotificatieGekozen)
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

        if (!bNotificatieGekozen)
        {
            return "Kies een notificatievoorkeur.";
        }

        if (!string.IsNullOrWhiteSpace(strGsm) && strGsm.Trim().Length > 20)
        {
            return "Het gsm-nummer is te lang (max. 20 tekens).";
        }

        return null;
    }
}
