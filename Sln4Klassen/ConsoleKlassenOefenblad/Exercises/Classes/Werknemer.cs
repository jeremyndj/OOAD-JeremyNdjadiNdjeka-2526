using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    internal class Werknemer
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public decimal Salaris 
        {
            get { return field; } //Je zou ook gewoon " get; " hebben kunnen laten staan als je er niets mee doet
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Salaris kan niet negatief zijn");
                }
                field = value;
            }
        }
        public DateOnly InDienstSinds { get; set; }
        public int Ancienniteit { get; set; }
        public string Seniority { get; set; }
    }
}
