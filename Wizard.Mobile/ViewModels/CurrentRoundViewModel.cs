using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace Wizard.Mobile.ViewModels
{
    public class CurrentRoundViewModel : BindableBase
    {
        public string RoundText { get; }
        public int RoundNumber { get; }
        public IReadOnlyList<RoundResultViewModel> Scores { get; }

        public CurrentRoundViewModel(int roundNumber, ICollection<PlayerViewModel> players)
        {
            RoundNumber = roundNumber;
            RoundText = $"{roundNumber}/{60 / players.Count}";

            Scores = players.Select(x => x.RoundViewModels[roundNumber - 1]).ToList();
            if (!Scores.Any(x => x.IsDealer))
            {
                Scores[(roundNumber - 1) % Scores.Count].IsDealer = true;
            }
            isCompleted = true;
            Calculate();
        }

        public void Calculate()
        {
            TotalBet = Scores.Sum(x => x.Bet);
            TotalResult = Scores.Sum(x => x.Result);
            IsCompleted = TotalResult == RoundNumber;
        }

        private bool isCompleted;
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (SetProperty(ref isCompleted, value))
                {
                    foreach (var score in Scores)
                    {
                        score.IsCompleted = isCompleted;
                    }
                }
            }
        }

        private int totalBet;
        public int TotalBet
        {
            get => totalBet;
            set => SetProperty(ref totalBet, value);
        }

        private int totalResult;
        public int TotalResult
        {
            get => totalResult;
            private set => SetProperty(ref totalResult, value);
        }
    }
}