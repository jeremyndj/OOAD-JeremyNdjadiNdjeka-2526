using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    internal class Recept
    {
        public string Titel { get; set; }
        public int Rating { get; set; }
        public bool IsVegetarisch {  get; set; }
        public List<string> Ingrediënten {  get; set; } = new List<string>();
    }
}
