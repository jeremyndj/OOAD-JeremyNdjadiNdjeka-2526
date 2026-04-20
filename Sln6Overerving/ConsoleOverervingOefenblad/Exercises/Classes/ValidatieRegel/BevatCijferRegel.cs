using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel
{
    internal class BevatCijferRegel : ValidatieRegel
    {
        public override string FoutBoodschap => $"Waarde moet een cijfer bevatten";
        public override bool IsGeldig(string waarde)
        {
            if (string.IsNullOrEmpty(waarde))
                return false;

            return waarde.Any(char.IsDigit);
        }
    }
}
