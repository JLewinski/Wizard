using System.Collections.Generic;
using System.Linq;
using Wizard.Models;

namespace Wizard.Services
{
    public class DataService
    {
        private readonly string _connectionString;

        public DataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //TODO: Use paging
        public List<Game> GetGames()
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var collection = db.GetCollection<Game>(nameof(Game));
            return collection.Query().ToList();
        }

        //TODO: Use paging
        public List<GameSummary> GetSummary()
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var collection = db.GetCollection<GameSummary>(nameof(GameSummary));
            return collection.Query().ToList().OrderByDescending(x => x.LastUpdated).ToList();
        }

        public Game Create(IEnumerable<string> playerNames)
        {
            var game = new Game
            {
                Players = playerNames.Select(x => new Player { Name = x }).ToList(),
                LastUpdated = System.DateTime.Now
            };

            using var db = new LiteDB.LiteDatabase(_connectionString);

            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));

            var summary = new GameSummary(game.Id, game.Name, game.LastUpdated, false);

            var inserted = gameCollection.Insert(game);
            if (inserted > 0)
            {
                summary.Id = game.Id;
                inserted = summaryCollection.Insert(summary);
                if (inserted > 0)
                {
                    return game;
                }
                else
                {
                    _ = gameCollection.Delete(game.Id);
                }
            }

            return null;
        }

        public bool Save(Game game)
        {
            game.LastUpdated = System.DateTime.Now;

            using var db = new LiteDB.LiteDatabase(_connectionString);

            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));
            var totalRounds = 60 / game.Players.Count;
            var isFinished = game.Players.First().Rounds.Count == totalRounds && game.Players.Sum(x => x.Rounds.Last().Result) == totalRounds;
            var summary = new GameSummary(game.Id, game.Name, game.LastUpdated, isFinished);

            if (gameCollection.Update(game))
            {
                if (summaryCollection.Update(summary))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Save(IPoco<Game> vm) => Save(vm.ToPoco());

        public Game GetGame(int id)
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var collection = db.GetCollection<Game>(nameof(Game));
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public bool Delete(int id)
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));

            return gameCollection.Delete(id) && summaryCollection.Delete(id);
        }
    }
}
