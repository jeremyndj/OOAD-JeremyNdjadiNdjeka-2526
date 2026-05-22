using CLDokterspraktijk.Models;

namespace WpfPatiënt;

// =============================================================================
// ProfielWeergaveHelper — databasecodes naar leesbare UI-tekst
// =============================================================================
// ProfielPage alleen-lezen; geslacht 0/1/2 en Notificaties-enum.
// =============================================================================
public static class ProfielWeergaveHelper
{
    // -------------------------------------------------------------------------
    // VertaalGeslacht — code uit tabel Patient
    // -------------------------------------------------------------------------
    public static string VertaalGeslacht(string strCode)
    {
        if (strCode == "0")
        {
            return "Vrouw";
        }

        if (strCode == "1")
        {
            return "Man";
        }

        if (strCode == "2")
        {
            return "Anders";
        }

        return "Code " + strCode;
    }

    // -------------------------------------------------------------------------
    // VertaalNotificatie — enum naar Nederlandse label
    // -------------------------------------------------------------------------
    public static string VertaalNotificatie(Patient.Notificaties notificatie)
    {
        if (notificatie == Patient.Notificaties.Geen)
        {
            return "Geen";
        }

        if (notificatie == Patient.Notificaties.Mail)
        {
            return "E-mail";
        }

        if (notificatie == Patient.Notificaties.Sms)
        {
            return "Sms";
        }

        if (notificatie == Patient.Notificaties.Beide)
        {
            return "E-mail en sms";
        }

        return notificatie.ToString();
    }
}
