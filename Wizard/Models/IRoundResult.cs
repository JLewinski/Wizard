namespace Wizard.Models
{
    public interface IRoundResult
    {
        int Bet { get; set; }
        int Result { get; set; }
        bool IsDealer { get; set; }
        int RoundNumber { get; init; }

        // RoundResult ToPoco();
    }

    public class RoundResult : IRoundResult
    {
        public RoundResult() { }

        public int Bet { get; set; }
        public int Result { get; set; }
        public bool IsDealer { get; set; }
        public int RoundNumber { get; init; }
    }
}