using CLDokterspraktijk.Models;

namespace WpfPatiënt;

// Vertaling van databasecodes naar leesbare tekst op profielpagina's.
public static class ProfielWeergaveHelper
{
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
