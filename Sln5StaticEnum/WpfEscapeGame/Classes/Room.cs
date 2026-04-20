using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfEscapeGame.Classes
{
    internal class Room : Actor
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Doors> Doors { get; set; } = new List<Doors>();
        public Doors ToDoor { get; set; }
        public string Image {  get; set; }
    }
}
