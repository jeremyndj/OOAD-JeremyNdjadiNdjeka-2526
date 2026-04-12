namespace ConsoleOverervingOefenblad.Exercises.Classes.CatalogusItem;

internal class FilmDvd : CatalogusItem
{
    public string Titel { get; set; } = string.Empty;
    public string InventarisNummer { get; set; } = string.Empty;
    public string Regisseur { get; set; } = string.Empty;
    public int SpeelduurMinuten { get; set; }
}
