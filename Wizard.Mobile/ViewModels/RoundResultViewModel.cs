using Wizard.Models;
using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public class RoundResultViewModel : BindableBase, IRoundResult
    {
        private readonly GameViewModel _game;

        public RoundResultViewModel() { }
        public RoundResultViewModel(PlayerViewModel player, GameViewModel game, RoundResult poco) : this(player, game)
        {
            bet = poco.Bet;
            result = poco.Result;
            isDealer = poco.IsDealer;

            IsCompleted = true;

            _game = game;
        }

        public RoundResultViewModel(PlayerViewModel player, GameViewModel game)
        {
            Player = player;

            IsCompleted = false;

            _game = game;
        }

        #region View Properties

        public Color NameColor => isDealer ? ColorHelper.DEFAULT_COLOR : Color.Default;

        public Color NameTextColor => isDealer ? Color.White : Color.Default;

        public Color ResultColor => !_game.CurrentRound.IsCompleted ? Color.Default : bet == result ? ColorHelper.SUCCESS_GREEN : ColorHelper.DANGER_RED;

        public void UpdateView()
        {
            RaisePropertyChanged(nameof(ResultColor));
            Player.UpdatePoints();
        }

        private bool isCompleted;
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    Player.UpdatePoints();
                    RaisePropertyChanged(nameof(ResultColor));
                }
            }
        }

        #endregion View Properties

        #region IRoundResult

        public PlayerViewModel Player { get; }

        private int bet;
        public int Bet
        {
            get => bet; set
            {
                if (SetProperty(ref bet, value))
                {
                    _game?.UpdateAll();
                }
            }
        }

        private int result;
        public int Result
        {
            get => result; set
            {
                if (SetProperty(ref result, value))
                {
                    _game?.UpdateAll();
                }
            }
        }

        private bool isDealer = false;
        public bool IsDealer
        {
            get => isDealer;
            set
            {
                if (SetProperty(ref isDealer, value))
                {
                    RaisePropertyChanged(nameof(NameColor));
                }
            }
        }

        public RoundResult ToPoco() => new RoundResult
        {
            Bet = Bet,
            IsDealer = IsDealer,
            Result = Result
        };

        #endregion IRoundResult

    }
}