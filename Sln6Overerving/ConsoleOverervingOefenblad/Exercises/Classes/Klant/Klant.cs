namespace ConsoleOverervingOefenblad.Exercises.Classes.Klant;

internal class Klant
{
    public string Naam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Naam} ({Email})";
    }
}
