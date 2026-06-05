using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CLHelpdesk
{
    /// <summary>
    /// Basisklasse voor een helpdeskticket.
    /// Bevat gemeenschappelijke properties en CSV-logica.
    /// Afgeleide klassen: HardwareTicket en SoftwareTicket.
    /// </summary>
    public abstract class Ticket
    {
        private int id;
        private string titel;
        private Medewerker melder;
        private TicketPrioriteit prioriteit;
        private bool isAfgesloten;
        private DateTime datumAangemaakt;
        private DateTime? datumAfgesloten;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Titel
        {
            get { return titel; }
            set { titel = value; }
        }

        public Medewerker Melder
        {
            get { return melder; }
            set { melder = value; }
        }

        public TicketPrioriteit Prioriteit
        {
            get { return prioriteit; }
            set { prioriteit = value; }
        }

        public bool IsAfgesloten
        {
            get { return isAfgesloten; }
            set { isAfgesloten = value; }
        }

        public DateTime DatumAangemaakt
        {
            get { return datumAangemaakt; }
            set { datumAangemaakt = value; }
        }

        public DateTime? DatumAfgesloten
        {
            get { return datumAfgesloten; }
            set { datumAfgesloten = value; }
        }

        protected Ticket()
        {
            isAfgesloten = false;
            datumAangemaakt = DateTime.Now;
        }

        /// <summary>
        /// Geeft het type ticket als string (Hardware of Software).
        /// Wordt overschreven door afgeleide klassen.
        /// </summary>
        public abstract string GeefType();

        /// <summary>
        /// Geeft type-specifieke extra informatie voor de detailweergave.
        /// </summary>
        public abstract string GeefExtraInfoLabel();

        /// <summary>
        /// Geeft de waarde van het type-specifieke veld (toestel of applicatie).
        /// </summary>
        public abstract string GeefExtraInfoWaarde();

        /// <summary>
        /// Uitgebreide informatie voor de detailweergave in de UI.
        /// </summary>
        public virtual string GeefInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Ticket #" + id);
            sb.AppendLine("Titel: " + titel);
            sb.AppendLine("Type: " + GeefType());
            sb.AppendLine(GeefExtraInfoLabel() + ": " + GeefExtraInfoWaarde());
            sb.AppendLine("Melder: " + melder.ToString());
            sb.AppendLine("Prioriteit: " + prioriteit);
            sb.AppendLine("Status: " + (isAfgesloten ? "Afgesloten" : "Open"));
            sb.AppendLine("Aangemaakt: " + FormateerDatum(datumAangemaakt));

            if (datumAfgesloten != null)
            {
                sb.AppendLine("Afgesloten: " + FormateerDatum(datumAfgesloten.Value));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Korte weergave voor de ListBox in de UI.
        /// </summary>
        public override string ToString()
        {
            string status = isAfgesloten ? "Afgesloten" : "Open";
            return "#" + id + " | " + titel + " | " + GeefType()
                + " | " + prioriteit + " | " + status;
        }

        /// <summary>
        /// Zet het ticket om naar één CSV-regel (met aanhalingstekens en puntkomma's).
        /// </summary>
        public string NaarCsvRij()
        {
            string afgeslotenTekst = "";
            if (datumAfgesloten != null)
            {
                afgeslotenTekst = FormateerDatum(datumAfgesloten.Value);
            }

            string rij = id + ";" + titel + ";" + melder.Voornaam + ";" + melder.Achternaam + ";"
                + melder.Id + ";" + prioriteit + ";" + isAfgesloten.ToString().ToLower()
                + ";" + GeefType() + ";" + GeefExtraInfoWaarde() + ";"
                + FormateerDatum(datumAangemaakt) + ";" + afgeslotenTekst;

            return "\"" + rij + "\"";
        }

        /// <summary>
        /// Maakt een Ticket-object op basis van één CSV-datarij.
        /// Kiest automatisch HardwareTicket of SoftwareTicket op basis van het type-veld.
        /// </summary>
        public static Ticket MaakVanCsvRij(string rij)
        {
            string schoneRij = rij.Trim();
            if (schoneRij.StartsWith("\"") && schoneRij.EndsWith("\""))
            {
                schoneRij = schoneRij.Substring(1, schoneRij.Length - 2);
            }

            string[] velden = schoneRij.Split(';');

            int ticketId = int.Parse(velden[0]);
            string ticketTitel = velden[1];
            string voornaam = velden[2];
            string achternaam = velden[3];
            string melderId = velden[4];
            TicketPrioriteit prioriteit = ParsePrioriteit(velden[5]);
            bool afgesloten = bool.Parse(velden[6]);
            string type = velden[7];
            string extraInfo = velden[8];
            DateTime aangemaakt = ParseDatum(velden[9]);

            DateTime? afgeslotenDatum = null;
            if (velden.Length > 10 && !string.IsNullOrWhiteSpace(velden[10]))
            {
                afgeslotenDatum = ParseDatum(velden[10]);
            }

            Medewerker melder = new Medewerker(melderId, voornaam, achternaam);

            Ticket ticket = null;
            if (type == "Hardware")
            {
                HardwareTicket hw = new HardwareTicket();
                hw.Toestel = extraInfo;
                ticket = hw;
            }
            else if (type == "Software")
            {
                SoftwareTicket sw = new SoftwareTicket();
                sw.Applicatie = extraInfo;
                ticket = sw;
            }
            else
            {
                throw new ArgumentException("Onbekend tickettype: " + type);
            }

            ticket.Id = ticketId;
            ticket.Titel = ticketTitel;
            ticket.Melder = melder;
            ticket.Prioriteit = prioriteit;
            ticket.IsAfgesloten = afgesloten;
            ticket.DatumAangemaakt = aangemaakt;
            ticket.DatumAfgesloten = afgeslotenDatum;

            return ticket;
        }

        /// <summary>
        /// Laadt alle tickets uit een CSV-bestand.
        /// De eerste rij (kolomkoppen) wordt overgeslagen.
        /// </summary>
        public static List<Ticket> LaadAlleUitBestand(string bestandPad)
        {
            List<Ticket> tickets = new List<Ticket>();

            if (!File.Exists(bestandPad))
            {
                return tickets;
            }

            string[] regels = File.ReadAllLines(bestandPad);

            for (int i = 1; i < regels.Length; i++)
            {
                string regel = regels[i].Trim();
                if (string.IsNullOrWhiteSpace(regel))
                {
                    continue;
                }

                Ticket ticket = MaakVanCsvRij(regel);
                tickets.Add(ticket);
            }

            return tickets;
        }

        /// <summary>
        /// Bewaart alle tickets naar een CSV-bestand.
        /// Schrijft eerst de kolomkoppen, daarna elke ticketrij.
        /// </summary>
        public static void BewaarAlleNaarBestand(string bestandPad, List<Ticket> tickets)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("id;titel;melderVoornaam;melderAchternaam;melderId;prioriteit;isAfgesloten;type;extraInfo;datumAangemaakt;datumAfgesloten");

            foreach (Ticket ticket in tickets)
            {
                sb.AppendLine(ticket.NaarCsvRij());
            }

            File.WriteAllText(bestandPad, sb.ToString());
        }

        /// <summary>
        /// Parseert een prioriteitswaarde uit de CSV (Laag, Normaal, Hoog).
        /// </summary>
        private static TicketPrioriteit ParsePrioriteit(string waarde)
        {
            if (waarde == "Laag")
            {
                return TicketPrioriteit.Laag;
            }
            if (waarde == "Normaal")
            {
                return TicketPrioriteit.Normaal;
            }
            if (waarde == "Hoog")
            {
                return TicketPrioriteit.Hoog;
            }

            throw new ArgumentException("Onbekende prioriteit: " + waarde);
        }

        /// <summary>
        /// Parseert een datum uit het CSV-formaat (bijv. 2026-04-20 0915).
        /// </summary>
        private static DateTime ParseDatum(string waarde)
        {
            return DateTime.ParseExact(waarde.Trim(), "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formatteert een datum naar het CSV-formaat (bijv. 2026-04-20 0915).
        /// </summary>
        protected static string FormateerDatum(DateTime datum)
        {
            return datum.ToString("yyyy-MM-dd HHmm", CultureInfo.InvariantCulture);
        }
    }
}
