using System.Collections.Generic;
using System.Linq;
using Wizard.Extensions;

namespace Wizard.Models
{
    public class Player : IPlayer
    {
        public Player() { }

        public string Name { get; set; }
        private List<RoundResult> _rounds = new List<RoundResult>();
        public IEnumerable<IRoundResult> Rounds
        {
            get => _rounds;
            set => _rounds = value.Select(x => x.ToPoco()).ToList();
        }

        public void AddRound()
        {
            var roundNumber = _rounds.Max(x => x.RoundNumber) + 1;
            _rounds.Add(new RoundResult{ RoundNumber = roundNumber });
        }
    }
}