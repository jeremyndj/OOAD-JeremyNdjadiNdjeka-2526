using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal class Dokter: Persoon
    {
        public int Rizivinummer {  get; set; }
        public bool Isgeconventioneerd { get; set; }
    }
}
