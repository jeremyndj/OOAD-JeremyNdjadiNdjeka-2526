using System.Collections.Generic;
using System.Text;

namespace CLHelpdesk
{
    /// <summary>
    /// Medewerker die een ticket kan aanmelden.
    /// Komt overeen met melderId, melderVoornaam en melderAchternaam in het CSV-bestand.
    /// </summary>
    public class Medewerker
    {
        private string id;
        private string voornaam;
        private string achternaam;
        private List<Ticket> tickets;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Voornaam
        {
            get { return voornaam; }
            set { voornaam = value; }
        }

        public string Achternaam
        {
            get { return achternaam; }
            set { achternaam = value; }
        }

        /// <summary>
        /// Alle tickets die door deze medewerker zijn aangemeld.
        /// </summary>
        public List<Ticket> Tickets
        {
            get { return tickets; }
            set { tickets = value; }
        }

        public Medewerker()
        {
            tickets = new List<Ticket>();
        }

        public Medewerker(string id, string voornaam, string achternaam)
        {
            this.id = id;
            this.voornaam = voornaam;
            this.achternaam = achternaam;
            tickets = new List<Ticket>();
        }

        /// <summary>
        /// Volledige naam van de medewerker.
        /// </summary>
        public override string ToString()
        {
            return voornaam + " " + achternaam + " (" + id + ")";
        }
    }
}
