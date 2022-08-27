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
}