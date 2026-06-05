using System.Text;

namespace CLHelpdesk
{
    /// <summary>
    /// Ticket voor softwareproblemen (Teams, Outlook, Excel, ...).
    /// Komt overeen met type 'Software' in het CSV-bestand.
    /// Het veld 'extraInfo' uit CSV wordt opgeslagen als Applicatie.
    /// </summary>
    public class SoftwareTicket : Ticket
    {
        private string applicatie;

        public string Applicatie
        {
            get { return applicatie; }
            set { applicatie = value; }
        }

        public override string GeefType()
        {
            return "Software";
        }

        public override string GeefExtraInfoLabel()
        {
            return "Applicatie";
        }

        public override string GeefExtraInfoWaarde()
        {
            return applicatie;
        }

        /// <summary>
        /// Uitgebreide info inclusief applicatiegegevens.
        /// </summary>
        public override string GeefInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.GeefInfo());
            return sb.ToString();
        }
    }
}
