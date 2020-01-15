
using System.Collections.Generic;

namespace Model.Domain
{
    public class Room : Entity
    {
        public string Name { get; set; }
        public string HexColor { get; set; }
        public string NormalizeName { get; set; }

        public List<ClassTime> Schedule { get; set; }

        public Room WithName(string name)
        {
            Name = name;
            NormalizeName = name.ToUpperInvariant();
            return this;
        }

        public Room WithColor(string color)
        {
            HexColor = color;
            return this;
        }
    }
}
