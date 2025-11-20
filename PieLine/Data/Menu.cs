using System.Collections.Generic;

namespace PieLine
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Image { get; set; }
    }
}
