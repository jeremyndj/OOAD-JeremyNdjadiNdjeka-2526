namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel;

internal class BevatHoofdletterRegel : ValidatieRegel
{
    public bool IsGeldig(string waarde) => waarde.Any(char.IsUpper);

    public string FoutBoodschap => "Waarde moet minstens één hoofdletter bevatten.";
}
