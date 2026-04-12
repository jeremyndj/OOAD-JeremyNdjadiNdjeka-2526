using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfEscapeGame.Classes
{
    internal class Doors
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOpen { get; set; }
        public bool KeyFits { get; set; }
        public Room ToRoom { get; set; }
    }
}
