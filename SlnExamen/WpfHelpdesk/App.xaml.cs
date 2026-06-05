using System.IO;
using System.Windows;
using CLHelpdesk;

namespace WpfHelpdesk
{
    /// <summary>
    /// Application entry point.
    /// Initialiseert het gedeelde TicketBeheer-object met het pad naar het CSV-bestand.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gedeeld ticketbeheer voor alle vensters. Geen CSV-logica in de UI.
        /// </summary>
        public static TicketBeheer Beheer { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            string csvPad = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "helpdesk_tickets.csv");

            Beheer = new TicketBeheer(csvPad);
            base.OnStartup(e);
        }
    }
}
