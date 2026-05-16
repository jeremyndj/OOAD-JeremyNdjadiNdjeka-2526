using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal abstract class Persoon
    {
        public int Id { get; set; }
        public string Voornaam {  get; set; }
        public string Achternaam { get; set; }
        public string Geslacht {  get; set; }
        public string Gsm { get; set; }
        public string  Email { get; set; }
        public string Passwoord { get; set; }

    }
}
