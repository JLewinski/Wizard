using System;
using System.Collections.Generic;
using System.Linq;
using Wizard.Extensions;

namespace Wizard.Models
{
    public class Game : IGame
    {
        public Game()
        {
            Suits = new List<Suit>();
        }

        public Game(List<string> playerNames)
        {
            Players = playerNames.Select(x => new Player { Name = x } as IPlayer).ToList();
            LastUpdated = System.DateTime.Now;
        }

        public Game(int id, string name, IEnumerable<IPlayer> players, List<Suit> suits)
        {
            Id = id;
            Name = name;
            Players = players.ToList();
            Suits = suits;
        }

        public static Game Create(List<string> playerNames)
        {
            return new Game(playerNames);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        private List<Player> _players;
        public List<IPlayer> Players { get => _players.Cast<IPlayer>().ToList(); set => _players = value.Select(x => x.ToPoco()).ToList(); }

        public List<Suit> Suits { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; }
    }
}