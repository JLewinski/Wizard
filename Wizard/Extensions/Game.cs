using System.Collections.Generic;
using System.Linq;
using Wizard.Models;
using Wizard.Services;

namespace Wizard.Extensions
{
    public static class GameExtension
    {
        public static (bool success, int roundNumber, IReadOnlyList<string> errors) NextRound(this IGame game, int roundNumber, bool save, IDataService dataService = null)
        {
            var errors = new List<string>();
            var totalResult = game.Players.Sum(x => x.Rounds.ElementAt(roundNumber - 1).Result);
            if (roundNumber != 60 / game.Players.Count)
            {
                if (roundNumber > 0 && roundNumber != totalResult)
                {
                    save = false;
                    errors.Add("results != round number");
                }
                if (roundNumber == game.Players.Max(x => x.Rounds.Max(y => y.RoundNumber)))
                {
                    roundNumber++;
                    foreach (var player in game.Players.Where(x => x.Rounds.Count() <= roundNumber))
                    {
                        player.AddRound();
                    }
                }
            }
            else
            {
                //TODO: Game finished
            }

            if (save)
            {
                if (dataService?.Save(game) != true)
                {
                    errors.Add("Could not save the game");
                }
            }

            return (!errors.Any(), roundNumber, errors);
        }
    }
}