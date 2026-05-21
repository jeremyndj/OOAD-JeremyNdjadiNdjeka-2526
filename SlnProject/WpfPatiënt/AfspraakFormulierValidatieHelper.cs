namespace WpfPatiënt;

// UI-validatie voor AfspraakMakenPage vóór AfspraakService.MaakAfspraak (geen database hier).
public static class AfspraakFormulierValidatieHelper
{
    // Controleert dokter, datum, tijd en reden; retourneert fouttekst of null als alles in orde is.
    public static string? ValideerFormulier(int iDokterId, DateTime? datumGekozen, string strTijd, string strKlacht)
    {
        if (iDokterId <= 0)
        {
            return "Kies een dokter.";
        }

        if (datumGekozen == null)
        {
            return "Kies een datum.";
        }

        if (string.IsNullOrWhiteSpace(strTijd))
        {
            return "Kies een tijdstip.";
        }

        if (string.IsNullOrWhiteSpace(strKlacht))
        {
            return "Vul een reden van consultatie in.";
        }

        DateTime? moment = BerekenMoment(datumGekozen.Value, strTijd.Trim());
        if (moment == null)
        {
            return "Het gekozen tijdstip is ongeldig.";
        }

        if (moment.Value <= DateTime.Now)
        {
            return "Kies een datum en tijdstip in de toekomst.";
        }

        return null;
    }

    // Combineert datum (datumdeel) en tijdtekst HH:mm tot één DateTime; null bij parsefout.
    public static DateTime? BerekenMoment(DateTime datumDag, string strTijd)
    {
        string[] arrDelen = strTijd.Split(':');
        if (arrDelen.Length != 2)
        {
            return null;
        }

        int iUur;
        int iMinuut;
        if (!int.TryParse(arrDelen[0], out iUur))
        {
            return null;
        }

        if (!int.TryParse(arrDelen[1], out iMinuut))
        {
            return null;
        }

        if (iUur < 0 || iUur > 23 || iMinuut < 0 || iMinuut > 59)
        {
            return null;
        }

        DateTime moment = datumDag.Date.AddHours(iUur).AddMinutes(iMinuut);
        return moment;
    }
}
