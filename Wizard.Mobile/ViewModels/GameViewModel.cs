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
    public class GameViewModel : BindableBase, IPoco<Game>
    {
        private readonly string _dbPath;
        private readonly Services.DataService _dataService;
        public readonly IReadOnlyList<Suit> SuitChoices = (Enum.GetValues(typeof(Suit)) as Suit[]).ToList();
        private readonly Dictionary<int, CurrentRoundViewModel> _rounds = new Dictionary<int, CurrentRoundViewModel>();

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
        }

        #region Commands 

        public ICommand NextRoundCommand { get; }
        public ICommand PreviousRoundCommand { get; }
        public ICommand NewGameCommand { get; }

        #endregion Commands

        #region Load

        public void Load(Game game)
        {
            if (game != null)
            {
                id = game.Id;
                name = game.Name;
                foreach (var player in game.Players)
                {
                    PlayerViewModels.Add(new PlayerViewModel(player, this));
                }

                int maxRoundNumber = PlayerViewModels.Any(x => x.RoundViewModels.Any(y => y.Result > 0)) ? PlayerViewModels[0].RoundViewModels.Count : 0;
                for (int roundNumber = 1; roundNumber <= maxRoundNumber; roundNumber++)
                {
                    _rounds.Add(roundNumber, new CurrentRoundViewModel(roundNumber, PlayerViewModels));
                }

                if (maxRoundNumber > 0)
                {
                    CurrentRound = _rounds[maxRoundNumber];
                    RaisePropertyChanged(nameof(TotalStatusColor));
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

        #endregion Load

        #region Methods

        public void NextRound(bool save = true)
        {
            int roundNumber = CurrentRound?.RoundNumber ?? 0;
            if (roundNumber != 60 / PlayerViewModels.Count)
            {
                if (roundNumber > 0 && roundNumber != CurrentRound.TotalResult)
                {
                    //Show error saying results != round number
                    return;
                }

                foreach (var player in PlayerViewModels.Where(x => x.RoundViewModels.Count <= roundNumber))
                {
                    player.AddRound();
                }

                roundNumber++;

                if (_rounds.TryGetValue(roundNumber, out _currentRound))
                {
                    RaisePropertyChanged(nameof(CurrentRound));
                }
                else
                {
                    CurrentRound = new CurrentRoundViewModel(roundNumber, PlayerViewModels);
                    _rounds.Add(roundNumber, CurrentRound);
                }
            }

            UpdateAll();

            if (save)
            {
                _ = _dataService.Save(this);
            }
        }

        public void PreviousRound()
        {
            if (CurrentRound?.RoundNumber <= 1)
            {
                return;
            }

            CurrentRound = _rounds[CurrentRound.RoundNumber - 1];
            UpdateAll();
        }

        public void UpdateAll()
        {
            CurrentRound.Calculate();
            RaisePropertyChanged(nameof(TotalStatusColor));
        }

        #endregion Methods

        #region View Properties

        public Color TotalStatusColor => CanGoNextRound ? ColorHelper.SUCCESS_GREEN : Color.Transparent;

        public bool CanGoNextRound => CurrentRound?.IsCompleted ?? false;

        private CurrentRoundViewModel _currentRound;
        public CurrentRoundViewModel CurrentRound { get => _currentRound; set => SetProperty(ref _currentRound, value); }

        public ObservableCollection<PlayerViewModel> PlayerViewModels { get; set; } = new ObservableCollection<PlayerViewModel>();

        #endregion View Properties

        #region Implement IGame

        private int id;
        private string name;
        public ICollection<Suit> Suits { get; set; } = new ObservableCollection<Suit>();

        public Game ToPoco()
        {
            return new Game()
            {
                Id = id,
                Name = name,
                Players = PlayerViewModels.Select(x => x.ToPoco()).ToList(),
                Suits = Suits.ToList()
            };
        }

        #endregion Implement IGame


    }
}