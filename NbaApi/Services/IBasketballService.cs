using NbaApi.Models;

namespace NbaApi.Services;

public interface IBasketballService
{
    Task<List<Team?>> GetTeamsWithOrderedPlayers();
}