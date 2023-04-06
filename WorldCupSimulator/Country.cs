using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupSimulator
{
    public class Country
    {
        string name, colormain, colorsecondary, group;

        public string Name { get => name; set => name = value; }
        public string Colormain { get => colormain; set => colormain = value; }
        public string Colorsecondary { get => colorsecondary; set => colorsecondary = value; }
        public string Group { get => group; set => group = value; }

        public Country(string name, string colormain, string colorsecondary, string group)
        {
            Name = name;
            Colormain = colormain;
            Colorsecondary = colorsecondary;
            Group = group;
        }
    }
}
