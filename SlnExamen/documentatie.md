# Documentatie — Helpdesk Applicatie

**Student:** Jeremy Ndjadi Ndjeka — 2de bachelor Toegepaste Informatica, Odisee  
**Project:** Interne IT-helpdesk (WPF + CSV)

---

## 1. Initiële prompt

We moeten een eenvoudige helpdesk-applicatie bouwen voor een interne IT-helpdesk. Medewerkers melden problemen met hardware of software. Een helpdeskmedewerker kan tickets raadplegen, filteren, nieuwe tickets registreren en tickets afsluiten.

Verplichte projectbestanden in de root:
- Een **agent instruction file** (`AGENTS.md`) met regels voor AI (geen data binding, LINQ, var, async/await, tuples, enz.; alle logica in een class library).
- Een **documentatie.md** met initiële prompt, plan van aanpak, gebruikte agents en gesprekssamenvattingen.

Technische keuzes:
- **WPF** (`WpfHelpdesk`) + **class library** (`CLHelpdesk`)
- **CSV-bestand** (`helpdesk_tickets.csv`) als opslag, geen database
- Scheidingsteken: puntkomma (`;`)

---

## 2. Plan van aanpak

### Architectuur

```
WpfHelpdesk (UI)  →  CLHelpdesk (domein + CSV)  →  helpdesk_tickets.csv
```

### Domeinmodel (CLHelpdesk)

| Type | Naam | Beschrijving |
|------|------|--------------|
| Enum | `TicketPrioriteit` | Laag, Normaal, Hoog |
| Klasse | `Medewerker` | Id, Voornaam, Achternaam, lijst Tickets |
| Basisklasse | `Ticket` | Id, Titel, Melder, Prioriteit, IsAfgesloten, datums; `GeefInfo()`, `ToString()` |
| Afgeleid | `HardwareTicket` | Extra property `Toestel` |
| Afgeleid | `SoftwareTicket` | Extra property `Applicatie` |
| Beheer | `TicketBeheer` | Laden/opslaan CSV, toevoegen, afsluiten, filteren |

CSV-kolommen in `helpdesk_tickets.csv`:
`id`, `titel`, `melderVoornaam`, `melderAchternaam`, `melderId`, `prioriteit`, `isAfgesloten`, `type`, `extraInfo`, `datumAangemaakt`, `datumAfgesloten`

### UI-schermen (WpfHelpdesk)

1. **MainWindow** — rolkeuze (medewerker / helpdeskmedewerker)
2. **MedewerkerWindow** — nieuw ticket aanmaken (vrije invoer meldergegevens)
3. **HelpdeskWindow** — links: filters + checkbox + scrollbare ticketlijst; rechts: details, afsluiten, nieuw ticket met melder uit CSV

Geen data binding; alle UI-updates handmatig in code-behind. Validatiefouten als rode tekst in het scherm.

---

## 3. Gebruikte agents

| Agent | Doel |
|-------|------|
| Cursor Agent (Plan mode) | Eerste plan van aanpak opgesteld (later aangepast naar CSV + CLHelpdesk) |
| Cursor Agent (Agent mode) | Volledige implementatie, layout-aanpassingen en eindcontrole |

---

## 4. Gespreksverloop

### Sessie 1 — 5 juni 2026

**Doel:** Plan opstellen voor helpdesk-applicatie met AGENTS.md en documentatie.md.

**Resultaat:** Plan gemaakt met WPF + JSON. Gebruiker koos WPF en JSON-persistentie.

### Sessie 2 — 5 juni 2026

**Doel:** Aanpassing aan exameneisen: CSV i.p.v. JSON, projectnamen CLHelpdesk/WpfHelpdesk, OOAD-klassenstructuur met overerving.

**Resultaat:** Domeinmodel vastgelegd (TicketPrioriteit, Medewerker, Ticket, HardwareTicket, SoftwareTicket). CSV-kolommen gekoppeld aan properties. Afgesproken: CSV-logica enkel in class library, geen aparte datalayer.

### Sessie 3 — 5 juni 2026

**Doel:** Volledige implementatie van CLHelpdesk en WpfHelpdesk.

**Resultaat:**
- `CLHelpdesk`: alle modellen, CSV-logica in `Ticket`, beheer in `TicketBeheer`
- `WpfHelpdesk`: drie vensters (rolkeuze, medewerker, helpdesk)
- Geen data binding, LINQ of `var`; build geslaagd

### Sessie 4 — 5 juni 2026

**Doel:** Layout HelpdeskWindow verbeteren en validatie via rode fouttekst.

**Resultaat:**
- Linkerkolom: filters onder elkaar, checkbox "Alleen open tickets", scrollbare ticketlijst
- Rechterkolom: detailblok, knop afsluiten, nieuw-ticketformulier
- Melder-ComboBox gevuld met medewerkers uit CSV
- Validatiefouten als rode tekst; regel in `AGENTS.md`

### Sessie 5 — 5 juni 2026

**Doel:** Eindcontrole voor indiening.

**Resultaat:**
- Volledige checklist doorlopen (zie sectie 5)
- `MedewerkerWindow` aangepast: ook geen MessageBox meer, rode/groene tekstmeldingen
- `AGENTS.md` en `documentatie.md` aangevuld
- Build geslaagd, project klaar voor indiening

---

## 5. Eindcontrole — checklist indiening

| Vereiste | Status |
|----------|--------|
| `AGENTS.md` in projectroot | ✅ |
| `documentatie.md` in projectroot | ✅ |
| Initiële prompt in documentatie | ✅ |
| Plan van aanpak in documentatie | ✅ |
| Gebruikte agents + gesprekssamenvattingen | ✅ |
| Class library `CLHelpdesk` | ✅ |
| WPF-app `WpfHelpdesk` | ✅ |
| CSV-bestand `helpdesk_tickets.csv` in root | ✅ |
| Geen database | ✅ |
| Enum `TicketPrioriteit` | ✅ |
| Klasse `Medewerker` met lijst Tickets | ✅ |
| Basisklasse `Ticket` met `GeefInfo()` en `ToString()` | ✅ |
| `HardwareTicket` en `SoftwareTicket` (overerving) | ✅ |
| CSV-logica enkel in class library | ✅ |
| Geen aparte datalayer/datacontext | ✅ |
| Medewerker meldt tickets aan | ✅ |
| Helpdesk raadpleegt, filtert, registreert, sluit af | ✅ |
| Geen data binding | ✅ |
| Geen LINQ, var, async/await, tuples, ListView, DataGrid | ✅ |
| Validatiefouten als rode tekst (geen MessageBox) | ✅ |
| Rijkelijk commentaar in class library | ✅ |
| Solution buildt zonder errors | ✅ |
