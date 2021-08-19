using LiteDB;
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
        [BsonIgnore]
        public ICommand NextRoundCommand => _nextRoundCommand;

        private readonly ICommand _previousRoundCommand;
        [BsonIgnore]
        public ICommand PreviousRoundCommand => _previousRoundCommand;

        private readonly ICommand _saveCommand;
        [BsonIgnore]
        public ICommand SaveCommand => _saveCommand;

        private readonly ICommand _newGameCommand;
        [BsonIgnore]
        public ICommand NewGameCommand => _newGameCommand;

        private readonly ICommand _newPlayerCommand;
        [BsonIgnore]
        public ICommand NewPlayerCommand => _newPlayerCommand;

        private readonly ICommand _clearPlayersCommand;
        [BsonIgnore]
        public ICommand ClearPlayersCommand => _clearPlayersCommand;

        public void NextRound()
        {
            var roundNumber = CurrentRound?.RoundNumber ?? 0;
            if (roundNumber != 60 / Players.Count)
            {
                if(roundNumber > 0 && roundNumber != TotalResult)
                {
                    //Show error saying results != round number
                    return;
                }

                if (Rounds.Count <= roundNumber)
                {
                    Rounds.Add(new RoundViewModel(Players, roundNumber + 1, this));
                }
                CurrentRound = _rounds[roundNumber];
                GameStarted = true;
            }
            _previousRound = roundNumber;
            CalculateScores();
            UpdateAll();
        }

        public void PreviousRound()
        {
            if (CurrentRound?.RoundNumber <= 1)
            {
                return;
            }
            CurrentRound = _rounds[CurrentRound.RoundNumber - 2];
            UpdateAll();
        }

        public void Clear()
        {
            _rounds.Clear();
            GameStarted = false;
            ScoresVisible = true;
            _previousRound = 0;
            CurrentRound = null;
            foreach(var player in Players)
            {
                player.Points = 0;
            }
        }

        public void Load()
        {
            Clear();
            using (var db = new LiteDatabase(_dbPath))
            {
                var games = db.GetCollection<Game>(nameof(GameViewModel));
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
                var games = db.GetCollection<GameViewModel>(nameof(GameViewModel));
                if (!games.Update(this))
                {
                    games.Insert(this);
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
                        score -= 10 * System.Math.Abs(result.Bet - result.Result);
                    }
                }
                changed = changed || player.Points != score;
                player.Points = score;
            }
            Save();
        }

        private bool _gameStarted = false;
        [BsonIgnore]
        public bool GameStarted { get => _gameStarted; set { SetProperty(ref _gameStarted, value); RaisePropertyChanged(nameof(GameNotStarted)); } }

        [BsonIgnore]
        public bool GameNotStarted => !_gameStarted;

        private bool _scoresVisible = true;
        [BsonIgnore]
        public bool ScoresVisible { get => _scoresVisible; set => SetProperty(ref _scoresVisible, value); }

        private IRound _currentRound;
        [BsonIgnore]
        public IRound CurrentRound { get => _currentRound; set => SetProperty(ref _currentRound, value); }

        private int _totalBid;
        [BsonIgnore]
        public int TotalBid { get => _totalBid; private set => SetProperty(ref _totalBid, value); }

        private int _totalResult;
        [BsonIgnore]
        public int TotalResult { get => _totalResult;
            private set
            {
                if(SetProperty(ref _totalResult, value))
                {
                    RaisePropertyChanged(nameof(TotalStatusColor));
                    if (CanGoNextRound)
                    {
                        foreach(var p in CurrentRound.Results)
                        {
                            var vm = p as RoundResultViewModel;
                            vm.ShowColor();
                        }
                    }
                }
            }
        }

        private const double RBG = 255;

        private static Color ColorHelperRGB(int colorHex)
        {
            double b = (byte)colorHex / RBG;
            double g = (byte)(colorHex >> 08) / RBG;
            double r = (byte)(colorHex >> 16) / RBG;

            return new Color(r, g, b);
        }

        private static Color ColorHelperRGBA(int colorHex)
        {
            double a = (byte)colorHex / RBG;
            double b = (byte)(colorHex >> 08) / RBG;
            double g = (byte)(colorHex >> 16) / RBG;
            double r = (byte)(colorHex >> 24) / RBG;

            return new Color(r, g, b, a);
        }

        public static readonly Color DEFAULT_COLOR = ColorHelperRGB(0x2196F3);

        [BsonIgnore]
        public Color TotalStatusColor => CanGoNextRound ? Color.Green : DEFAULT_COLOR;

        [BsonIgnore]
        public bool CanGoNextRound => !_gameStarted || TotalResult == CurrentRound.RoundNumber;

        private ObservableCollection<IPlayer> _players;
        public ICollection<IPlayer> Players { get => _players; set => SetProperty(ref _players, new ObservableCollection<IPlayer>(value.Select(x => new PlayerViewModel(x)))); }

        private ObservableCollection<IRound> _rounds;
        public ICollection<IRound> Rounds { get => _rounds; set => SetProperty(ref _rounds, new ObservableCollection<IRound>(value.Select(x => new RoundViewModel(x, this)))); }

        public int NumRounds => (Players?.Count ?? 0) / 60;

        public int Id { get; set; } = 1;

        public void UpdateAll()
        {
            UpdateTotalBets();
            UpdateTotalResults();
            RaisePropertyChanged(nameof(TotalStatusColor));
        }

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
            Results[(RoundNumber - 1) % Results.Count].IsDealer = true;
        }

        public RoundViewModel(IRound other, GameViewModel game)
        {
            RoundNumber = other.RoundNumber;
            Suit = other.Suit;
            Results = new ObservableCollection<IRoundResult>(other.Results.Select(x => new RoundResultViewModel(x, game)));
            Results[(RoundNumber - 1) % Results.Count].IsDealer = true;
        }

        private Suit _suit;
        public Suit Suit { get => _suit; set => SetProperty(ref _suit, value); }
        public int RoundNumber { get; set; }
        public Collection<IRoundResult> Results { get; set; }
    }

    public class RoundResultViewModel : BindableBase, IRoundResult
    {
        public RoundResultViewModel() { }
        public RoundResultViewModel(IRoundResult other, GameViewModel game)
        {
            _playerName = other.PlayerName;
            _bet = other.Bet;
            _result = other.Result;
            _game = game;
        }
        public RoundResultViewModel(string playerName, GameViewModel game)
        {
            PlayerName = playerName;
            _game = game;
        }

        private string _playerName;
        public string PlayerName { get => _playerName; set => SetProperty(ref _playerName, value); }

        private int _bet;
        public int Bet
        {
            get => _bet; set
            {
                if (SetProperty(ref _bet, value))
                {
                    _game?.UpdateTotalBets();
                }
            }
        }

        private int _result;
        public int Result
        {
            get => _result; set
            {
                if (SetProperty(ref _result, value))
                {
                    _game?.UpdateTotalResults();
                }
            }
        }

        private bool isDealer = false;
        public bool IsDealer
        {
            get => isDealer;
            set
            {
                if(SetProperty(ref isDealer, value))
                {
                    RaisePropertyChanged(nameof(NameColor));
                }
            }
        }

        [BsonIgnore]
        public Color NameColor => isDealer ? GameViewModel.DEFAULT_COLOR : Color.Default;
        public Color NameTextColor => isDealer ? Color.White : Color.Default;

        public void ShowColor() => RaisePropertyChanged(nameof(ResultColor));

        [BsonIgnore]
        public Color ResultColor => !_game.CanGoNextRound ? Color.Default : _bet == _result ? Color.Green : Color.Red;

        private readonly GameViewModel _game;
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