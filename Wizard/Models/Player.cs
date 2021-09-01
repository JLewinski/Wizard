using System.Collections.Generic;

namespace Wizard.Models
{
    public class Player
    {
        public Player() { }

        public string Name { get; set; }
        public List<RoundResult> Rounds { get; set; } = new List<RoundResult>();
    }
}