using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wizard.Models;
using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public class GameViewModel : BindableBase, IGame
    {
        private readonly string _dbPath;
        private readonly Services.DataService _dataService;
        public readonly IReadOnlyList<Suit> SuitChoices = (Enum.GetValues(typeof(Suit)) as Suit[]).ToList();
        private readonly List<CurrentRoundViewModel> _rounds = new List<CurrentRoundViewModel>();
        
        private int id;

        public GameViewModel()
        {
            _dbPath = DependencyService.Get<IDataBaseAccess>().DatabasePath();
            _dataService = new Services.DataService(_dbPath);
            NextRoundCommand = new Command(() => NextRound());
            PreviousRoundCommand = new Command(PreviousRound);
            NewGameCommand = new Command(async () =>
            {
                var navPage = Application.Current.MainPage as NavigationPage;
                var popedPage = await navPage.PopAsync();
            });
            NewPlayerCommand = new Command(() =>
            {
                if (Players.Count < 6)
                {
                    Players.Add(new PlayerViewModel());
                }
            });
            ClearPlayersCommand = new Command(() => Players.Clear());
        }

        #region Commands 

        public ICommand NextRoundCommand { get; }
        public ICommand PreviousRoundCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand NewPlayerCommand { get; }
        public ICommand ClearPlayersCommand { get; }

        #endregion Commands

        #region Functional Methods

        public void NextRound(bool save = true)
        {
            var roundNumber = CurrentRound?.RoundNumber ?? 0;
            if (roundNumber != 60 / PlayerViewModels.Count)
            {
                if(roundNumber > 0 && roundNumber != TotalResult)
                {
                    //Show error saying results != round number
                    return;
                }

                foreach(var player in PlayerViewModels)
                {
                    if(player.RoundViewModels.Count <= roundNumber)
                    {
                        player.AddRound();
                    }
                }

                roundNumber++;

                CurrentRound = new CurrentRoundViewModel(roundNumber, PlayerViewModels);
                _rounds.Add(CurrentRound);
            }
            UpdateAll();
            if (save)
            {
                _dataService.Save(this);
            }
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

        public void Load(Game game)
        {
            if (game != null)
            {
                id = game.Id;
                foreach (var player in game.Players)
                {
                    PlayerViewModels.Add(new PlayerViewModel(player, this));
                }
                var roundNumber = PlayerViewModels.Any(x => x.RoundViewModels.Any(y => y.Result > 0)) ? PlayerViewModels[0].RoundViewModels.Count - 1 : 0;
                for (int i = 0; i < roundNumber; i++)
                {
                    _rounds.Add(new CurrentRoundViewModel(i + 1, PlayerViewModels));
                }

                if (roundNumber > 0)
                {
                    CurrentRound = new CurrentRoundViewModel(roundNumber, PlayerViewModels);
                    UpdateAll();
                }
                else
                {
                    NextRound(false);
                }
            }
        }

        public void Load(int id)
        {
            var game = _dataService.GetGame(id);
            Load(game);
        }

        #endregion Functional Methods

        #region View Properties

        private int _totalBid;
        public int TotalBid { get => _totalBid; private set => SetProperty(ref _totalBid, value); }

        private int _totalResult;
        public int TotalResult { get => _totalResult;
            private set
            {
                if(SetProperty(ref _totalResult, value))
                {
                    RaisePropertyChanged(nameof(TotalStatusColor));
                    if (CanGoNextRound)
                    {
                        foreach(var vm in CurrentRound.Scores)
                        {
                            vm.UpdateView();
                        }
                    }
                }
            }
        }

        public Color TotalStatusColor => CanGoNextRound ? ColorHelper.SUCCESS_GREEN : Color.Transparent;

        public bool CanGoNextRound => CurrentRound == null || TotalResult == CurrentRound.RoundNumber;

        private CurrentRoundViewModel _currentRound;
        public CurrentRoundViewModel CurrentRound { get => _currentRound; set => SetProperty(ref _currentRound, value); }

        public ObservableCollection<PlayerViewModel> PlayerViewModels { get; set; } = new ObservableCollection<PlayerViewModel>();

        #endregion View Properties

        #region Implement IGame

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public ICollection<IPlayer> Players => PlayerViewModels.Cast<IPlayer>().ToList();

        public ICollection<Suit> Suits { get; set; } = new ObservableCollection<Suit>();

        public Game ToPoco()
        {
            return new Game()
            {
                Id = id,
                Name = Name,
                Players = Players.Select(x => x.ToPoco()).ToList()
            };
        }

        #endregion Implement IGame

        #region View Methods

        public void UpdateAll()
        {
            UpdateTotalBets();
            UpdateTotalResults();
            RaisePropertyChanged(nameof(TotalStatusColor));
        }

        public void UpdateTotalBets() => TotalBid = PlayerViewModels.Sum(x => x.RoundViewModels[CurrentRound.RoundNumber - 1].Bet);

        public void UpdateTotalResults() => TotalResult = PlayerViewModels.Sum(x => x.RoundViewModels[CurrentRound.RoundNumber - 1].Result);

        #endregion View Methods
    }

    public class CurrentRoundViewModel
    {
        public string RoundText { get; }
        public int RoundNumber { get; }
        public IReadOnlyList<RoundResultViewModel> Scores { get; }

        public CurrentRoundViewModel(int roundNumber, ICollection<PlayerViewModel> players)
        {
            RoundNumber = roundNumber;
            RoundText = $"{roundNumber}/{60 / players.Count}";

            Scores = players.Select(x => x.RoundViewModels[roundNumber - 1]).ToList();
            if(!Scores.Any(x => x.IsDealer))
            {
                Scores[(roundNumber - 1) % Scores.Count].IsDealer = true;
            }
        }
    }
}