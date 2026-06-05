# Agent Instructions — Helpdesk Applicatie (OOAD)

Dit document bevat verplichte richtlijnen voor elke AI-agent die aan dit schoolproject werkt.

## Projectcontext

We bouwen een **helpdesk-applicatie** voor een schoolproject (2de bachelor Toegepaste Informatica, Odisee). De applicatie ondersteunt het beheer van supporttickets: aanmaken, opvolgen, filteren en afsluiten van meldingen door medewerkers en helpdeskmedewerkers.

## Architectuur

### Class library (verplicht)

- **Alle businesslogica en domeinklassen horen in `CLHelpdesk`**.
- De UI-laag (`WpfHelpdesk`) mag **alleen** de class library aanroepen; geen businesslogica in forms of windows.
- **CSV-logica** (lezen, schrijven, parsen) mag **enkel** in de class library staan, nooit in WPF code-behind.
- Gebruik **klassen** met duidelijke verantwoordelijkheden (SRP). Geen losse static helpers voor domeinlogica.
- Geen aparte datalayer, repository of datacontext — methodes om objecten te lezen/toe te voegen zitten in de domeinklassen zelf.
- Pas gangbare OOAD-patronen toe: encapsulation, inheritance, polymorfisme (`Ticket` → `HardwareTicket` / `SoftwareTicket`).

### Projectstructuur

```
SlnExamen/
├── CLHelpdesk/          ← class library (modellen, CSV, ticketbeheer)
├── WpfHelpdesk/         ← presentatielaag (windows)
├── helpdesk_tickets.csv ← databestand (puntkomma-gescheiden)
├── AGENTS.md
└── documentatie.md
```

## Verboden technieken en constructies

| Categorie | Verboden |
|-----------|----------|
| UI-binding | Data binding van welke aard dan ook |
| UI-controls | `DataGrid`, `GridView`, `ListView`, user controls |
| Query/taal | LINQ (geen `.Where()`, `.Select()`, `.First()`, enz.) |
| Types | `tuple` / `(T1, T2)`, `struct`, `dynamic`, `ExpandoObject` |
| Variabelen | `var` — gebruik altijd expliciete types |
| Asynchroon | `async` / `await` |
| Parameters | `out`-parameters |
| Reflectie | `Invoke` (reflection) |
| Pattern matching | `switch` expressions met type patterns, case guards |
| Overig | Type switches |

## Toegestane alternatieven

- **Geen LINQ** → gebruik `foreach`-lussen en expliciete `List<T>`-bewerkingen.
- **Geen DataGrid/ListView** → handmatig controls opbouwen of een eenvoudige `ListBox` zonder databinding.
- **Geen async/await** → synchrone methodes; I/O via `File.ReadAllText` / `File.WriteAllText`.
- **Geen var** → schrijf `string titel = ...` in plaats van `var titel = ...`.

## UI-regels

- **Validatiefouten** tonen als **rode tekst** in het scherm (`TextBlock` met `Foreground="Red"`).
- Gebruik **geen `MessageBox`** voor invoervalidatie, foutmeldingen of succesmeldingen bij formulieren.
- Succesmeldingen mogen als groene tekst in het scherm (`Foreground="Green"`).
- Na ticket toevoegen of afsluiten: overzicht **herladen** via de class library.
- Gebruik `ListBox` voor ticketlijsten; geen `ListView`, `DataGrid` of `GridView`.

## Codekwaliteit

- Duidelijke, Nederlandse of Engelstalige namen (consistent binnen het project).
- Properties met getters/setters; geen public fields.
- **Rijkelijk commentaar** bij niet-triviale logica (CSV-parsing, filtering, overerving).
- Exceptions alleen waar zinvol; geen over-engineered error handling.

## Documentatie

- Werk `documentatie.md` bij na elk relevant AI-gesprek.
- Verwijs naar gebruikte agents en het doel van het gesprek.
