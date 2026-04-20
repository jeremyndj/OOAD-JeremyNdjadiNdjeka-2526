namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel;

internal class BevatHoofdletterRegel : ValidatieRegel
{
    public override bool IsGeldig(string waarde)
    {
        if (string.IsNullOrEmpty(waarde))
            return false;

        return waarde.Any(char.IsUpper);
    }
    public override string FoutBoodschap => "Waarde moet minstens één hoofdletter bevatten.";
}
