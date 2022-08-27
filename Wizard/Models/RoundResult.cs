namespace Wizard.Models
{
    public class RoundResult : IRoundResult
    {
        public RoundResult() { }

        public int Bet { get; set; }
        public int Result { get; set; }
        public bool IsDealer { get; set; }
        public int RoundNumber { get; init; }
    }
}