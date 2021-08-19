using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wizard.Models
{
    public static class GameExtensions
    {
        public static Game ToPoco(this IGame game) => game is Game poco ? poco : new Game(game);
        //public static PlayerPoco ToPoco(this IPlayer player) => player is PlayerPoco poco ? poco : new PlayerPoco(player);
        //public static RoundPoco ToPoco(this IRound player) => player is RoundPoco poco ? poco : new RoundPoco(player);
        //public static RoundResultPoco ToPoco(this IRoundResult player) => player is RoundResultPoco poco ? poco : new RoundResultPoco(player);
    }

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

        Collection<IRoundResult> Results { get; set; }
    }

    public interface IRoundResult
    {
        string PlayerName { get; set; }
        int Bet { get; set; }
        int Result { get; set; }
        bool IsDealer { get; set; }
    }

    
}