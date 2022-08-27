using System.Collections.Generic;
using System.Threading.Tasks;
using Wizard.Models;

namespace Wizard.Services
{
    public class SqlService : IDataService
    {
        public Game Create(Game game)
        {
            throw new System.NotImplementedException();
        }

        public Task<Game> CreateAsync(Game game)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteGame(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteGameAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Game GetGame(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Game> GetGameAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<Game> GetGames()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Game>> GetGamesAsync()
        {
            throw new System.NotImplementedException();
        }

        public List<GameSummary> GetSummaries()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<GameSummary>> GetSummariesAsync()
        {
            throw new System.NotImplementedException();
        }

        public bool Save(Game game)
        {
            throw new System.NotImplementedException();
        }

        public bool Save(IGame vm)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SaveAsync(Game game)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SaveAsync(IGame vm)
        {
            throw new System.NotImplementedException();
        }
    }
}