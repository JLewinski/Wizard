using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wizard.Extensions;
using Wizard.Models;

namespace Wizard.Services
{
    public class LiteDBService : IDataService
    {
        private readonly string _connectionString;

        public LiteDBService(string connectionString)
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

        public Task<List<Game>> GetGamesAsync() => Task.Run(() => GetGames());

        //TODO: Use paging
        public List<GameSummary> GetSummaries()
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var collection = db.GetCollection<GameSummary>(nameof(GameSummary));
            return collection.Query().ToList().OrderByDescending(x => x.LastUpdated).ToList();
        }

        public Task<List<GameSummary>> GetSummariesAsync() => Task.Run(() => GetSummaries());

        public Game Create(Game game)
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);

            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));

            var summary = new GameSummary(game.Id, game.Name, game.DateCreated, game.LastUpdated, false);

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

        public Task<Game> CreateAsync(Game game) => Task.Run(() => Create(game));

        public bool Save(Game game)
        {
            game.LastUpdated = System.DateTime.Now;

            using var db = new LiteDB.LiteDatabase(_connectionString);

            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));
            var totalRounds = 60 / game.Players.Count;
            var isFinished = game.Players.First().Rounds.Count() == totalRounds && game.Players.Sum(x => x.Rounds.Last().Result) == totalRounds;
            var summary = new GameSummary(game.Id, game.Name, game.DateCreated, game.LastUpdated, isFinished);

            if (gameCollection.Update(game))
            {
                if (summaryCollection.Update(summary))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Save(IGame vm) => Save(vm.ToPoco());

        public Task<bool> SaveAsync(IGame vm) => Task.Run(() => Save(vm));

        public Game GetGame(int id)
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var collection = db.GetCollection<Game>(nameof(Game));
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public Task<Game> GetGameAsync(int id) => Task.Run(() => GetGame(id));

        public bool DeleteGame(int id)
        {
            using var db = new LiteDB.LiteDatabase(_connectionString);
            var gameCollection = db.GetCollection<Game>(nameof(Game));
            var summaryCollection = db.GetCollection<GameSummary>(nameof(GameSummary));

            return gameCollection.Delete(id) && summaryCollection.Delete(id);
        }

        public Task<bool> DeleteGameAsync(int id) => Task.Run(() => DeleteGame(id));
    }
}
