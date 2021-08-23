using System.Collections.Generic;

namespace Wizard.Models
{
    public interface IPlayer
    {
        string Name { get; }
        public IEnumerable<IRoundResult> Rounds { get; }

        public Player ToPoco();
    }
}