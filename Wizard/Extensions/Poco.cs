using Wizard.Models;

namespace Wizard.Extensions
{
    public static class PocoExtensions
    {
        public static RoundResult ToPoco(this IRoundResult roundResult)
        {
            return new RoundResult
            {
                Bet = roundResult.Bet,
                IsDealer = roundResult.IsDealer,
                Result = roundResult.Result
            };
        }

        public static Game ToPoco(this IGame game)
        {
            return new Game
            {
                DateCreated = game.DateCreated,
                Id = game.Id,
                LastUpdated = game.LastUpdated,
                Name = game.Name,
                Players = game.Players,
                Suits = game.Suits
            };
        }

        public static Player ToPoco(this IPlayer player)
        {
            return new Player
            {
                Name = player.Name,
                Rounds = player.Rounds
            };
        }
    }
}