using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchThePairs.Models
{
    public class GameState
    {
        public int TableSizeX { get; set; }
        public int TableSizeY { get; set; }
        public string CategoryName { get; set; }
        public string UserName { get; set; }
        public int TimerCount { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public List<CardState> Cards { get; set; }
        public int MatchesFound { get; set; }
        public int TotalPairs { get; set; }

        public GameState()
        {
            Cards = new List<CardState>();
        }
    }
}
