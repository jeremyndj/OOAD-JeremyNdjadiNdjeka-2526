# AI-instructies — Dokterspraktijk (OOAD)

## Codeconventies

- Bij **string properties** in klassen geen `= string.Empty` initializer gebruiken.
- Gebruik **`var` niet** — schrijf altijd het expliciete type (bijv. `List<Patient> patients = ...` in plaats van `var patients = ...`).

## Verboden technieken

- **Geen data binding** in WPF (geen `{Binding ...}`, geen `ItemsSource` koppelen aan observables/viewmodels voor automatische UI-updates).
- **Geen** `DataGrid`, `GridView` of `ListView` — gebruik andere controls (bijv. `ListBox`) en vul lijsten **handmatig** in code-behind (bijv. `Items.Add` of `foreach`).

_(Wordt geleidelijk aangevuld door het team.)_
