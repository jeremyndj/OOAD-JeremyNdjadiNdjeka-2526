# AI-instructies — Dokterspraktijk (OOAD)

## Codeconventies

- Bij **string properties** in klassen geen `= string.Empty` initializer gebruiken.
- Gebruik **`var` niet** — schrijf altijd het expliciete type (bijv. `List<Patient> lijstPatienten = ...` in plaats van `var lijstPatienten = ...`).
- Gebruik **Hongaarse notatie** voor namen van variabelen, parameters en UI-controls (`x:Name`):
  - Voorvoegsels: `btn` (Button), `txt` (TextBlock/TextBox), `img` (Image), `fra` (Frame), `pnl` (Panel), `lst` (ListBox), `cmb` (ComboBox), `str` (string), `i` (int), `b` (bool), `arr` (array), `bmp` (BitmapImage), `stm` (Stream), enz.
  - Voorbeeld: `btnAfspraken`, `txtGebruikersnaam`, `fraMain`, `arrProfielData`.
- **PascalCase** voor herbruikbare methodes (public/private methods die je aanroept), event handlers (`BtnAfspraken_Click`), en klassen/properties.
- **camelCase** niet gebruiken voor methodes — wel Hongaars + PascalCase voor methods: `LaadGebruikerInHeader`, `NavigeerNaarAfspraken`.

## Commentaar (verplichte stijl — altijd zo schrijven)

Gebruik **overal** in het project (WPF code-behind, class library, helpers, services, repositories) **dezelfde commentaarstijl** als in `WpfDokter` (o.a. `MainWindow.xaml.cs`, `PatientBewerkPage.xaml.cs`, `LoginPage.xaml.cs`). Bij nieuwe of aangepaste code **altijd** op deze manier commenteren — niet korter of alleen “eenvoudige” regels.

### Taal en diepgang

- Taal: **Nederlands**.
- **Rijkelijk en expliciet**: uitleg van *wat*, *waarom* en *hoe het in de app past* (data-flow, Session, SQL-laag, UI-gedrag).
- Niet alleen herhalen wat de code letterlijk doet (geen `// zet tekst` bij `txtNaam.Text = ...`).
- Geen nutteloze regel-voor-regel-commentaar bij triviale code, maar wél voldoende context zodat iemand de flow kan volgen zonder de hele solution te doorzoeken.

### Opbouw per bestand

1. **Klassenblok bovenaan** (direct onder `namespace`, vóór `public class`):
   - Regels met `// =============================================================================`
   - Titel: `// Klassenaam — korte omschrijving`
   - Daaronder: bullet-achtige regels met `//` over rol, navigatie, services, afspraken (geen binding, geen SQL in WPF, …).

2. **Velden**: korte `//` bij private fields die state of een afhankelijkheid uitleggen (bijv. service, vlag `_bInitialiseert`, byte-array profielfoto).

3. **Belangrijke methodes**:
   - Vóór de methode een blok `// -------------------------------------------------------------------------`
   - Regel `// Methodenaam — wat doet deze methode`
   - Optioneel extra `//`-regels: wanneer aangeroepen (XAML-event), stappen, randgevallen.
   - Bij lange methodes: **genummerde stappen** in commentaar (`// Stap 1: ...`, `// Stap 2: ...`).

4. **Korte private helpers**: minstens één regel `//` met doel (niet alleen de methodenaam).

### Waar expliciet over schrijven (indien van toepassing)

- Scheiding UI ↔ `CLDokterspraktijk` (geen SQL in WPF).
- Formvalidatie: **txtFout** vs **MessageBox** (formchecking geen MessageBox, tenzij bevestiging zoals afspraak annuleren).
- `Session`, `NavigationService`, `Tag` op knoppen, constructor-parameters (`id == 0` = nieuw).
- Disabled knoppen vs verborgen menu; INSERT vs UPDATE.
- Databasecodes (bijv. geslacht 0/1/2).

### Voorbeeld (klassenkop — zo moet het eruitzien)

```csharp
// =============================================================================
// PatientBewerkPage — toevoegen en wijzigen op één formulier
// =============================================================================
// id 0 = INSERT; id > 0 = UPDATE. Validatie via PatientFormulierValidatieHelper en txtFout.
// Profielfoto optioneel; bij nieuwe patiënt blijft Opslaan disabled tot formulier compleet is.
// =============================================================================
public partial class PatientBewerkPage : Page
```

### Voorbeeld (methodekop)

```csharp
// -------------------------------------------------------------------------
// BtnOpslaan_Click — validatie, dan INSERT of UPDATE, daarna navigatie
// -------------------------------------------------------------------------
// Geen opslaan bij validatiefout: melding in txtFout, geen MessageBox.
private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
```

### Scope

- **Verplicht** bij: alle `.xaml.cs` (pages, MainWindow), repositories, services, security/helpers in `CLDokterspraktijk` en `WpfDokter`.
- Bij bestaande code die je aanpast: commentaar in deze stijl **bijwerken of aanvullen**, niet alleen nieuwe regels zonder structuur.

## Data-access

- **SQL-queries** (SELECT, INSERT, UPDATE, DELETE) horen **alleen** in de class library (`CLDokterspraktijk`), bijvoorbeeld in repositories. **Geen** SQL in WPF-projecten (geen query-strings in pages, code-behind of vensters).
- Geen businesslogica in code-behind: alleen UI-events die services aanroepen.

## Verboden technieken

Gebruik de volgende **niet** in nieuwe of aangepaste code (ook niet in voorbeelden of refactors):

| Verboden | Toelichting / alternatief |
|----------|---------------------------|
| **LINQ** | Geen `using System.Linq`, geen `.Where`, `.Select`, `.First`, `.Any`, enz. Gebruik `foreach`, `for` en expliciete `List<T>`. |
| **Tuples** | Geen `(int, string)` return types, geen tuple-deconstructie, geen discard-parameters `(_, _)` in lambdas. Gebruik named parameters `(object sender, RoutedEventArgs e)` of aparte methodes met `Tag` op controls. |
| **Case guard / pattern matching** | Geen `is Type naam`, geen `switch` met `when`, geen switch-expressions met `_`. Gebruik `as` + `null`-check of klassieke `if`/`else`/`switch` op primitieve waarden. |
| **async / await** | Alles synchroon houden (ADO.NET, UI-events). |
| **dynamic** | Altijd expliciete types. |
| **var** | Altijd expliciet type declareren. |
| **ExpandoObject** | Niet gebruiken. |
| **Invoke** | Geen `Method.Invoke` (reflectie) en geen `Dispatcher.Invoke` tenzij de opdracht dat expliciet vraagt — bij voorkeur gewone event handlers. |
| **structs** | Alleen `class` voor eigen types (behalve framework-types). |
| **Type switches** | Geen `switch` op runtime-type van objecten. |
| **User controls** | Geen custom `UserControl`-componenten; UI via `Window`, `Page` en standaard WPF-controls in XAML. |
| **out parameters** | Geen `out` in methodesignatures; gebruik returnwaarde of een object dat je vult. |

### Al eerder afgesproken (WPF / data)

- **Geen data binding** in WPF (geen `{Binding ...}`, geen `ItemsSource` koppelen aan observables/viewmodels voor automatische UI-updates).
- **Geen** `DataGrid`, `GridView` of `ListView` — gebruik andere controls (bijv. `ListBox`, `WrapPanel`) en vul lijsten **handmatig** in code-behind (bijv. `Items.Add` of `foreach`).
- **Geen** Entity Framework, Dapper of ORM — alleen ADO.NET (`SqlConnection`, `SqlCommand`, parameters) in `CLDokterspraktijk`.
- **Geen** hardcoded wachtwoorden in broncode; wachtwoord-hash alleen via `PasswordHasher` en database.

_(Overige teamafspraken kunnen hieronder worden aangevuld.)_
