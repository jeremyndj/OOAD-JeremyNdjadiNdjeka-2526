using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal class Patiënt : Persoon
    {
        public DateTime geboortedatum {  get; set; }
        public Notificaties notificaties { get; set; }
    }
}
