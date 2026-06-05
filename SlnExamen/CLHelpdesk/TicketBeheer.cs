using System;
using System.Collections.Generic;

namespace CLHelpdesk
{
    /// <summary>
    /// Beheert de collectie tickets en alle bewerkingen (laden, opslaan, toevoegen, afsluiten, filteren).
    /// Alle CSV-werkzaamheden lopen via de Ticket-klasse; deze klasse coördineert de operaties.
    /// Geen aparte datalayer — dit is domeinlogica in de class library.
    /// </summary>
    public class TicketBeheer
    {
        private List<Ticket> tickets;
        private string csvPad;
        private int volgendeId;

        public TicketBeheer(string csvPad)
        {
            this.csvPad = csvPad;
            tickets = new List<Ticket>();
            LaadUitCsv();
        }

        /// <summary>
        /// Laadt tickets uit het CSV-bestand via Ticket.LaadAlleUitBestand.
        /// </summary>
        public void LaadUitCsv()
        {
            tickets = Ticket.LaadAlleUitBestand(csvPad);
            volgendeId = BepaalHoogsteId() + 1;
            KoppelTicketsAanMedewerkers();
        }

        /// <summary>
        /// Bewaart alle tickets naar het CSV-bestand via Ticket.BewaarAlleNaarBestand.
        /// </summary>
        public void BewaarNaarCsv()
        {
            Ticket.BewaarAlleNaarBestand(csvPad, tickets);
        }

        /// <summary>
        /// Voegt een nieuw ticket toe en slaat op naar CSV.
        /// </summary>
        public void VoegTicketToe(Ticket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Titel))
            {
                throw new ArgumentException("Titel is verplicht.");
            }

            ticket.Id = volgendeId;
            volgendeId = volgendeId + 1;
            ticket.DatumAangemaakt = DateTime.Now;
            ticket.IsAfgesloten = false;
            ticket.DatumAfgesloten = null;

            tickets.Add(ticket);
            KoppelTicketAanMedewerker(ticket);
            BewaarNaarCsv();
        }

        /// <summary>
        /// Sluit een ticket af op basis van het id.
        /// </summary>
        public void SluitTicketAf(int ticketId)
        {
            Ticket gevonden = ZoekTicketOpId(ticketId);

            if (gevonden == null)
            {
                throw new InvalidOperationException("Ticket niet gevonden.");
            }

            if (gevonden.IsAfgesloten)
            {
                throw new InvalidOperationException("Ticket is al afgesloten.");
            }

            gevonden.IsAfgesloten = true;
            gevonden.DatumAfgesloten = DateTime.Now;
            BewaarNaarCsv();
        }

        /// <summary>
        /// Geeft alle tickets terug.
        /// </summary>
        public List<Ticket> GetAlleTickets()
        {
            return tickets;
        }

        /// <summary>
        /// Geeft een ticket terug op basis van het id.
        /// </summary>
        public Ticket GetTicketOpId(int ticketId)
        {
            return ZoekTicketOpId(ticketId);
        }

        /// <summary>
        /// Filtert tickets op prioriteit, type, afgesloten-status en zoektekst.
        /// Gebruikt foreach-lussen (geen LINQ).
        /// </summary>
        /// <param name="prioriteitFilter">Null = geen filter op prioriteit.</param>
        /// <param name="typeFilter">Leeg, "Hardware" of "Software".</param>
        /// <param name="afgeslotenFilter">Null = alles, true = alleen afgesloten, false = alleen open.</param>
        /// <param name="zoektekst">Zoekt in titel en extra info.</param>
        public List<Ticket> FilterTickets(TicketPrioriteit? prioriteitFilter, string typeFilter,
            bool? afgeslotenFilter, string zoektekst)
        {
            List<Ticket> result = new List<Ticket>();

            foreach (Ticket ticket in tickets)
            {
                bool match = true;

                if (prioriteitFilter != null && ticket.Prioriteit != prioriteitFilter)
                {
                    match = false;
                }

                if (!string.IsNullOrWhiteSpace(typeFilter))
                {
                    if (typeFilter == "Hardware" && !(ticket is HardwareTicket))
                    {
                        match = false;
                    }
                    if (typeFilter == "Software" && !(ticket is SoftwareTicket))
                    {
                        match = false;
                    }
                }

                if (afgeslotenFilter != null && ticket.IsAfgesloten != afgeslotenFilter)
                {
                    match = false;
                }

                if (!string.IsNullOrWhiteSpace(zoektekst))
                {
                    string zoekLower = zoektekst.ToLower();
                    bool titelBevat = ticket.Titel.ToLower().Contains(zoekLower);
                    bool extraBevat = ticket.GeefExtraInfoWaarde().ToLower().Contains(zoekLower);

                    if (!titelBevat && !extraBevat)
                    {
                        match = false;
                    }
                }

                if (match)
                {
                    result.Add(ticket);
                }
            }

            return result;
        }

        /// <summary>
        /// Bouwt een lijst van unieke medewerkers op basis van de geladen tickets.
        /// Elk Medewerker-object bevat een lijst van zijn/haar tickets.
        /// </summary>
        public List<Medewerker> GetMedewerkers()
        {
            List<Medewerker> medewerkers = new List<Medewerker>();

            foreach (Ticket ticket in tickets)
            {
                Medewerker bestaand = ZoekMedewerkerOpId(medewerkers, ticket.Melder.Id);

                if (bestaand == null)
                {
                    Medewerker nieuw = new Medewerker(
                        ticket.Melder.Id,
                        ticket.Melder.Voornaam,
                        ticket.Melder.Achternaam);
                    nieuw.Tickets.Add(ticket);
                    medewerkers.Add(nieuw);
                }
                else
                {
                    bestaand.Tickets.Add(ticket);
                }
            }

            return medewerkers;
        }

        /// <summary>
        /// Koppelt alle tickets aan hun melder in de Medewerker.Tickets-lijst.
        /// </summary>
        private void KoppelTicketsAanMedewerkers()
        {
            foreach (Ticket ticket in tickets)
            {
                KoppelTicketAanMedewerker(ticket);
            }
        }

        /// <summary>
        /// Voegt een ticket toe aan de Tickets-lijst van de melder.
        /// </summary>
        private void KoppelTicketAanMedewerker(Ticket ticket)
        {
            if (ticket.Melder.Tickets == null)
            {
                ticket.Melder.Tickets = new List<Ticket>();
            }

            bool alAanwezig = false;
            foreach (Ticket t in ticket.Melder.Tickets)
            {
                if (t.Id == ticket.Id)
                {
                    alAanwezig = true;
                    break;
                }
            }

            if (!alAanwezig)
            {
                ticket.Melder.Tickets.Add(ticket);
            }
        }

        /// <summary>
        /// Zoekt een ticket op id in de interne lijst.
        /// </summary>
        private Ticket ZoekTicketOpId(int ticketId)
        {
            foreach (Ticket ticket in tickets)
            {
                if (ticket.Id == ticketId)
                {
                    return ticket;
                }
            }

            return null;
        }

        /// <summary>
        /// Bepaalt het hoogste bestaande ticket-id.
        /// </summary>
        private int BepaalHoogsteId()
        {
            int max = 0;

            foreach (Ticket ticket in tickets)
            {
                if (ticket.Id > max)
                {
                    max = ticket.Id;
                }
            }

            return max;
        }

        /// <summary>
        /// Zoekt een medewerker op id in een lijst.
        /// </summary>
        private Medewerker ZoekMedewerkerOpId(List<Medewerker> medewerkers, string medewerkerId)
        {
            foreach (Medewerker medewerker in medewerkers)
            {
                if (medewerker.Id == medewerkerId)
                {
                    return medewerker;
                }
            }

            return null;
        }
    }
}
