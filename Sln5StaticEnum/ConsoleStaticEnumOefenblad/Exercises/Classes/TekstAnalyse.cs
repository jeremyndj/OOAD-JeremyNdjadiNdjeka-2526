using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{

    //  1. Maak in "Exercises/Classes" een statische class "TekstAnalyse":
    //  - private variabele verbodenWoorden: een array van strings met de woorden "delete", "drop", "truncate"
    //  - private variabele verbodenKarakters: een array van chars met de tekens "!", "@", "#", "$", "%"
    // 
    //  2. Voeg deze statische methodes toe:
    //  - AantalWoorden(string tekst): telt woorden op basis van spaties
    //  - BevatVerbodenWoord(string tekst): geeft true terug als de tekst verboden woorden bevat
    //  - BevatVerbodenKarakter(string tekst): geeft true terug als de tekst verboden karakters bevat
    //  - IsGeschiktVoorTitel(string tekst): geeft true terug als:
    //   * de tekst niet leeg is
    //   * de tekst minimum 5 en maximum 30 tekens lang is
    //   * de tekst geen verboden woorden of karakters bevat
    //
    //  Zorg ervoor dat de methodes ook veilig omgaan met null of lege strings.
    internal static class TekstAnalyse
    {
        private static readonly string[] verbodenWoorden = { "delete", "drop", "truncate" };
        private static readonly char[] verbodenKarakters = { '!', '@', '#', '$', '%' };

        public static int AantalWoorden(string tekst)
        {
            return 0;
        }
    }
}
