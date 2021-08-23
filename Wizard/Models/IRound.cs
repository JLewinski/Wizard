using System.Collections.ObjectModel;

namespace Wizard.Models
{
    public interface IRound
    {
        int RoundNumber { get; set; }
        int Bet { get; set; }
        int Result { get; set; }
        bool IsDealer { get; set; }
    }
}