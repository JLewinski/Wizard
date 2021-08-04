using System;
using System.Collections.Generic;

namespace Wizard.Models
{
    public class Game : IGame
    {
        public Game() { }
        public Game(IGame other)
        {
            Id = other.Id;
            Players = other.Players;
            Rounds = other.Rounds;
        }
        public int Id { get; set; }

        private List<IPlayer> _players;
        public ICollection<IPlayer> Players { get => _players; set => _players = new List<IPlayer>(value); }

        private List<IRound> _rounds;
        public ICollection<IRound> Rounds { get => _rounds; set => _rounds = new List<IRound>(value); }
    }

    public interface IGame
    {
        int Id { get; set;  }
        ICollection<IPlayer> Players { get; set;  }
        ICollection<IRound> Rounds { get; set;  }
    }

    public interface IPlayer
    {
        string Name { get; set; }
        int Points { get; set; }
    }

    public enum Suit
    {
        Hearts,
        Spades,
        Diamonds,
        Clubs
    }

    public interface IRound
    {
        int RoundNumber { get; set; }
        Suit Suit { get; set; }

        ICollection<IRoundResult> Results { get; set; }
    }

    public class Round : IRoundResult
    {
        public Round() { }
        public Round(IRoundResult other)
        {
            PlayerName = other.PlayerName;
        }

        public string PlayerName { get; set; }
        public int Bet { get; set; }
        public int Result { get; set; }
    }

    public interface IRoundResult
    {
        string PlayerName { get; set; }
        int Bet { get; set; }
        int Result { get; set; }
    }

    
}