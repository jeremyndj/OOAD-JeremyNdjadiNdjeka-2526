using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOverervingOefenblad.Exercises.Classes.Medewerker
{
    internal class Manager : Medewerker
    {
        public int TeamGrootte {  get; set; }
        public Manager(string naam, string afdeling, int Teamgrootte) 
            : base(naam, afdeling)
        {
            TeamGrootte = Teamgrootte;
        }

        public override string ToString()
        {
            if (TeamGrootte > 0)
                return $"{base.ToString}, team: {TeamGrootte} personen";
            else
                return base.ToString();
        }
    }
}
