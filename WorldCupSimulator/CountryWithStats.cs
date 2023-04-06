using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupSimulator
{
    public class CountryWithStats : Country
    {
        int wins, draws, losses, goals, points;

        public int Wins { get => wins; set => wins = value; }
        public int Draws { get => draws; set => draws = value; }
        public int Losses { get => losses; set => losses = value; }
        public int Goals { get => goals; set => goals = value; }
        public int Points { get => points; set => points = value; }

        public CountryWithStats(string name, string colormain, string colorsecondary, string group, int groupid, int wins, int draws, int losses, int goals, int points) : base(name, colormain, colorsecondary, group)
        {
            Wins = wins;
            Draws = draws;
            Losses = losses;
            Goals = goals;
            Points = points;
        }
    }
}
