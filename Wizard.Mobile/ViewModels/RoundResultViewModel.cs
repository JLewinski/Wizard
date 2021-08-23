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

            _game = game;
        }

        public RoundResultViewModel(PlayerViewModel player, GameViewModel game)
        {
            Player = player;
            _game = game;
        }

        #region View Properties

        public Color NameColor => isDealer ? ColorHelper.DEFAULT_COLOR : Color.Default;

        public Color NameTextColor => isDealer ? Color.White : Color.Default;

        public Color ResultColor => !_game.CanGoNextRound ? Color.Default : bet == result ? Color.Green : Color.Red;

        public void UpdateView()
        {
            RaisePropertyChanged(nameof(ResultColor));
            Player.UpdatePoints();
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
                    _game?.UpdateTotalBets();
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

        public RoundResult ToPoco() => new RoundResult
        {
            Bet = Bet,
            IsDealer= IsDealer,
            Result = Result
        };

        #endregion IRoundResult

    }
}