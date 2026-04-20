using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfEscapeGame.Classes
{
    internal class Doors : Actor
    {
        public bool IsLocked { get; set; }
        public Item Key { get; set; }
        public Room ToRoom { get; set; }
        
    }
}
