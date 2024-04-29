using System.Collections.Generic;
using football_functions.DTOs.Response;
using football_functions.Models;

namespace football_functions.Services.Interfaces;

public interface IDealer
{
    public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams, int numberOfPlayers, bool usePosition, Configs config);
}