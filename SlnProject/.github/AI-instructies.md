# AI-instructies — Dokterspraktijk (OOAD)

## Codeconventies

- Bij **string properties** in klassen geen `= string.Empty` initializer gebruiken.
- Gebruik **`var` niet** — schrijf altijd het expliciete type (bijv. `List<Patient> lijstPatienten = ...` in plaats van `var lijstPatienten = ...`).
- Gebruik **Hongaarse notatie** voor namen van variabelen, parameters en UI-controls (`x:Name`):
  - Voorvoegsels: `btn` (Button), `txt` (TextBlock/TextBox), `img` (Image), `fra` (Frame), `pnl` (Panel), `lst` (ListBox), `cmb` (ComboBox), `str` (string), `i` (int), `b` (bool), `arr` (array), `bmp` (BitmapImage), `stm` (Stream), enz.
  - Voorbeeld: `btnAfspraken`, `txtGebruikersnaam`, `fraMain`, `arrProfielData`.
- **PascalCase** voor herbruikbare methodes (public/private methods die je aanroept), event handlers (`BtnAfspraken_Click`), en klassen/properties.
- **camelCase** niet gebruiken voor methodes — wel Hongaars + PascalCase voor methods: `LaadGebruikerInHeader`, `NavigeerNaarAfspraken`.
- Voeg **eenvoudige commentaren** toe aan code die we schrijven (klassen, methodes en belangrijke stappen). Houd het kort en in het **Nederlands**. Geen overbodige uitleg van voor de hand liggende code (bijv. geen `// sluit venster` bij elke regel).

## Data-access

- **SQL-queries** (SELECT, INSERT, UPDATE, DELETE) horen **alleen** in de class library (`CLDokterspraktijk`), bijvoorbeeld in repositories. **Geen** SQL in WPF-projecten (geen query-strings in pages, code-behind of vensters).

## Verboden technieken

- **Geen data binding** in WPF (geen `{Binding ...}`, geen `ItemsSource` koppelen aan observables/viewmodels voor automatische UI-updates).
- **Geen** `DataGrid`, `GridView` of `ListView` — gebruik andere controls (bijv. `ListBox`) en vul lijsten **handmatig** in code-behind (bijv. `Items.Add` of `foreach`).

_(Wordt geleidelijk aangevuld door het team.)_
