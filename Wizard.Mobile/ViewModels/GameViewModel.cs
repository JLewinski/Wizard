using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Wizard.Models;
using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public class GameViewModel : BindableBase, IGame
    {
        private string _dbPath;
        private int _previousRound = 0;
        public IReadOnlyList<Suit> Suits = new List<Suit> { Suit.Spades, Suit.Diamonds, Suit.Clubs, Suit.Hearts };

        public GameViewModel()
        {
            _dbPath = DependencyService.Get<IDataBaseAccess>().DatabasePath();
            _players = new ObservableCollection<IPlayer>();
            _rounds = new ObservableCollection<IRound>();
            Load();

            _nextRoundCommand = new Command(NextRound);
            _previousRoundCommand = new Command(PreviousRound);
            _newGameCommand = new Command(Clear);
            _saveCommand = new Command(Save);
            _newPlayerCommand = new Command(() =>
            {
                if (Players.Count < 6)
                {
                    Players.Add(new PlayerViewModel());
                }
            });
            _clearPlayersCommand = new Command(() => Players.Clear());
        }

        private readonly ICommand _nextRoundCommand;
        public ICommand NextRoundCommand => _nextRoundCommand;

        private readonly ICommand _previousRoundCommand;
        public ICommand PreviousRoundCommand => _previousRoundCommand;

        private readonly ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private readonly ICommand _newGameCommand;
        public ICommand NewGameCommand => _newGameCommand;

        private readonly ICommand _newPlayerCommand;
        public ICommand NewPlayerCommand => _newPlayerCommand;

        private readonly ICommand _clearPlayersCommand;
        public ICommand ClearPlayersCommand => _clearPlayersCommand;

        public void NextRound()
        {
            var roundNumber = CurrentRound?.RoundNumber ?? 0;
            if (roundNumber != 60 / Players.Count)
            {
                if (Rounds.Count <= roundNumber)
                {
                    Rounds.Add(new RoundViewModel(Players, roundNumber + 1, this));
                }
                CurrentRound = _rounds[roundNumber];
                GameStarted = true;
            }
            _previousRound = roundNumber;
            CalculateScores();
        }

        public void PreviousRound()
        {
            if (CurrentRound?.RoundNumber <= 1)
            {
                return;
            }
            CurrentRound = _rounds[CurrentRound.RoundNumber - 2];
            CalculateScores();
        }

        public void Clear()
        {
            _rounds.Clear();
            GameStarted = false;
            ScoresVisible = true;
            _previousRound = 0;
            CurrentRound = null;
        }

        public void Load()
        {
            Clear();
            using (var db = new LiteDB.LiteDatabase(_dbPath))
            {
                var games = db.GetCollection<Game>(nameof(Game));
                var game = games.Query().FirstOrDefault();
                if (game != null)
                {
                    foreach (var player in game.Players)
                    {
                        Players.Add(player);
                    }
                    if (game.Rounds.Any(x => x.Results.Sum(y => y.Result) == x.RoundNumber))
                    {
                        foreach (var round in game.Rounds.OrderBy(x => x.RoundNumber).Select(x => new RoundViewModel(x, this)))
                        {
                            Rounds.Add(round);
                        }
                        GameStarted = true;
                        CurrentRound = Rounds.Last();
                    }
                }
            }
        }

        public void Save()
        {
            using (var db = new LiteDB.LiteDatabase(_dbPath))
            {
                var games = db.GetCollection<Game>(nameof(Game));
                var game = new Game(this);
                if (!games.Update(game))
                {
                    games.Insert(game);
                }
            }
        }

        public void CalculateScores()
        {
            bool changed = false;
            foreach (var player in Players)
            {
                var score = 0;

                foreach (var result in Rounds.Where(x => x.Results.Any(y => y.PlayerName == player.Name) && x.Results.Sum(y => y.Result) == x.RoundNumber).Select(x => x.Results.FirstOrDefault(y => y.PlayerName == player.Name)))
                {
                    if (result.Bet == result.Result)
                    {
                        score += 20 + (result.Bet * 10);
                    }
                    else
                    {
                        score -= 10 * Math.Abs(result.Bet - result.Result);
                    }
                }
                changed = changed || player.Points != score;
                player.Points = score;
            }
            Save();
        }

        private bool _gameStarted = false;
        public bool GameStarted { get => _gameStarted; set { SetProperty(ref _gameStarted, value); RaisePropertyChanged(nameof(GameNotStarted)); } }

        public bool GameNotStarted => !_gameStarted;

        private bool _scoresVisible = true;
        public bool ScoresVisible { get => _scoresVisible; set => SetProperty(ref _scoresVisible, value); }

        private IRound _currentRound;
        public IRound CurrentRound { get => _currentRound; set => SetProperty(ref _currentRound, value); }

        private int _totalBid;
        public int TotalBid { get => _totalBid; private set => SetProperty(ref _totalBid, value); }

        private int _totalResult;
        public int TotalResult { get => _totalResult; private set => SetProperty(ref _totalResult, value); }

        public bool CanGoNextRound => !_gameStarted || TotalResult == CurrentRound.RoundNumber;

        private ObservableCollection<IPlayer> _players;
        public ICollection<IPlayer> Players { get => _players; set => SetProperty(ref _players, new ObservableCollection<IPlayer>(value.Select(x => new PlayerViewModel(x)))); }

        private ObservableCollection<IRound> _rounds;
        public ICollection<IRound> Rounds { get => _rounds; set => SetProperty(ref _rounds, new ObservableCollection<IRound>(value.Select(x => new RoundViewModel(x, this)))); }

        public int NumRounds => (Players?.Count ?? 0) / 60;

        public int Id { get; set; } = 1;


        public void UpdateTotalBets() => TotalBid = CurrentRound?.Results.Sum(x => x.Bet) ?? 0;

        public void UpdateTotalResults() => TotalResult = CurrentRound?.Results.Sum(x => x.Result) ?? 0;
    }

    public class PlayerViewModel : BindableBase, IPlayer
    {
        public PlayerViewModel() { }
        public PlayerViewModel(IPlayer other)
        {
            _name = other.Name;
            _points = other.Points;
        }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private int _points;
        public int Points { get => _points; set => SetProperty(ref _points, value); }
    }

    public class RoundViewModel : BindableBase, IRound
    {
        public RoundViewModel() { }

        public RoundViewModel(IEnumerable<IPlayer> players, int roundNumber, GameViewModel game)
        {
            RoundNumber = roundNumber;
            Results = new ObservableCollection<IRoundResult>(players.Select(x => new RoundResultViewModel(x.Name, game)));
        }

        public RoundViewModel(IRound other, GameViewModel game)
        {
            RoundNumber = other.RoundNumber;
            Suit = other.Suit;
            Results = new ObservableCollection<IRoundResult>(other.Results.Select(x => new RoundResultViewModel(x, game)));
        }

        private Suit _suit;
        public Suit Suit { get => _suit; set => SetProperty(ref _suit, value); }
        public int RoundNumber { get; set; }
        public ICollection<IRoundResult> Results { get; set; }
    }

    public class RoundResultViewModel : BindableBase, IRoundResult
    {
        public RoundResultViewModel() { }
        public RoundResultViewModel(IRoundResult other, GameViewModel game)
        {
            _playerName = other.PlayerName;
            _bet = other.Bet;
            _result = other.Result;
            Game = game;
        }
        public RoundResultViewModel(string playerName, GameViewModel game)
        {
            PlayerName = playerName;
            Game = game;
        }

        public Delegate UpdateTotalBets { get; set; }
        public Delegate UpdateTotalResults { get; set; }

        private string _playerName;
        public string PlayerName { get => _playerName; set => SetProperty(ref _playerName, value); }

        private int _bet;
        public int Bet
        {
            get => _bet; set
            {
                SetProperty(ref _bet, value);
                Game.UpdateTotalBets();
            }
        }

        private int _result;
        public int Result { get => _result; set
            {
                SetProperty(ref _result, value);
                Game.UpdateTotalResults();
            }
        }

        public GameViewModel Game { get; set; }
    }

    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (property == null || !property.Equals(value))
            {
                property = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, args);
        }
    }
}