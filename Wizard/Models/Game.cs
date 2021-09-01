using System;
using System.Collections.Generic;
using System.Linq;

namespace Wizard.Models
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();
             Suits = new List<Suit>();
        }

        public Game(int id, string name, IEnumerable<IPlayer> players, List<Suit> suits)
        {
            Id = id;
            Name = name;
            Players = players.Select(x => x.ToPoco()).ToList();
            Suits = suits;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<Player> Players { get; set; }

        public List<Suit> Suits { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}