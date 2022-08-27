using System.Collections.Generic;
using System.Threading.Tasks;
using Wizard.Models;

namespace Wizard.Services
{
    public interface IDataService
    {
        List<Game> GetGames();
        Task<List<Game>> GetGamesAsync();
        List<GameSummary> GetSummaries();
        Task<List<GameSummary>> GetSummariesAsync();
        Game Create(Game game);
        Task<Game> CreateAsync(Game game);
        bool Save(IGame vm);
        Task<bool> SaveAsync(IGame vm);
        Game GetGame(int id);
        Task<Game> GetGameAsync(int id);
        bool DeleteGame(int id);
        Task<bool> DeleteGameAsync(int id);
    }
}