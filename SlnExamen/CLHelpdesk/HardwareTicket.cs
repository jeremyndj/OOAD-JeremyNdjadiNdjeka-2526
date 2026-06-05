using System.Text;

namespace CLHelpdesk
{
    /// <summary>
    /// Ticket voor hardwareproblemen (printer, laptop, muis, ...).
    /// Komt overeen met type 'Hardware' in het CSV-bestand.
    /// Het veld 'extraInfo' uit CSV wordt opgeslagen als Toestel.
    /// </summary>
    public class HardwareTicket : Ticket
    {
        private string toestel;

        public string Toestel
        {
            get { return toestel; }
            set { toestel = value; }
        }

        public override string GeefType()
        {
            return "Hardware";
        }

        public override string GeefExtraInfoLabel()
        {
            return "Toestel";
        }

        public override string GeefExtraInfoWaarde()
        {
            return toestel;
        }

        /// <summary>
        /// Uitgebreide info inclusief toestelgegevens.
        /// </summary>
        public override string GeefInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.GeefInfo());
            return sb.ToString();
        }
    }
}
