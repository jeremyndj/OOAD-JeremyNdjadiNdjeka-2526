using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

// 1. Maak in "Exercises/Classes" een enum "BestelStatus" met deze waarden:
//  - Nieuw
//  - InBehandeling
//  - Verzonden
//  - Geleverd
//  - Geannuleerd
//
// 2. Maak daarna een klasse "Bestelling" met:
//  - KlantNaam
//  - ProductNaam
//  - Status
//
// 3. Voeg ook een property KanNogGewijzigdWorden toe:
//  - niet elke status laat nog wijzigingen toe; zorg dus dat de methode enkel true teruggeeft wanneer dat logisch is.
//
// 4. Voeg een override van ToString() toe die de klantnaam, productnaam en bestelstatus netjes weergeeft (zie screenshot).
namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class Bestelling
    {
        public string KlantNaam { get; set; }
        public string ProductNaam { get; set; }
        public BestelStatus Status { get; set; }

        public bool KanNogGewijzigdWorden
        {
            get
            {
                if (Status == BestelStatus.Nieuw || Status == BestelStatus.InBehandeling )
                {
                    return true;
                }
                return false;
            }

        }


        public override string ToString()
        {
            string wijziging = KanNogGewijzigdWorden ? "ja" : "nee";
            return $"{ProductNaam} - {Status} - wijzigbaar:{wijziging}";
        }
    }
}
