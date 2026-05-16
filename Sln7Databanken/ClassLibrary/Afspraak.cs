using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal class Afspraak
    {
        public int Id { get; set; }
        public DateTime Moment { get; set; }
        public string Klacht { get; set; }
        public Patiënt Id { get; set; }
        public Dokter Id { get; set; }
    }
}
