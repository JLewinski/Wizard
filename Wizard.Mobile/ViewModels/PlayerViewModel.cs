using System.Collections.Generic;
using System.Linq;
using Wizard.Models;

namespace Wizard.Mobile.ViewModels
{
    public class PlayerViewModel : BindableBase, IPlayer
    {
        private readonly GameViewModel _game;

        public PlayerViewModel() { }
        public PlayerViewModel(Player other, GameViewModel game)
        {
            _name = other.Name;
            _game = game;
            RoundViewModels = other.Rounds.Select(x => new RoundResultViewModel(this, game, x)).ToList();
        }

        public void AddRound()
        {
            RoundViewModels.Add(new RoundResultViewModel(this, _game));
        }

        public void UpdatePoints()
        {
            RaisePropertyChanged(nameof(Points));
        }

        public int Points => RoundViewModels.Sum(x => x.Bet == x.Result ? 20 + (x.Result * 10) : System.Math.Abs(x.Bet - x.Result) * -10);

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public IEnumerable<IRoundResult> Rounds => RoundViewModels.Cast<IRoundResult>();

        private List<RoundResultViewModel> _rounds;
        public List<RoundResultViewModel> RoundViewModels
        {
            get => _rounds;
            set => SetProperty(ref _rounds, value);
        }

        public Player ToPoco() => new Player
        {
            Name = _name,
            Rounds = Rounds.Select(x => x.ToPoco()).ToList()
        };
    }
}