using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs.Request;
using football_functions.DTOs.Response;
using football_functions.Extensions;
using football_functions.Models;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class Teams
{
    private readonly IPlayerTableStorage _playerTableStorage;
    private readonly IDealer _dealer;
    private readonly IConfigTableStorage _configTableStorage;

    public Teams(IPlayerTableStorage playerTableStorage, IDealer dealer, IConfigTableStorage configTableStorage)
    {
        _playerTableStorage = playerTableStorage;
        _dealer = dealer;
        _configTableStorage = configTableStorage;
    }

    [FunctionName("Teams")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        var playersDTO = playersEntity.Select(p => p.ToPlayerDTO())
                                      .OrderByDescending(p => p.Score)
                                      .ToList();

        var getTeams = await JsonSerializer.DeserializeAsync<GetTeams>(req.Body);
        var selectedPlayerIds = getTeams.Ids;
        playersDTO = playersDTO.Where(p => selectedPlayerIds.Contains(p.Id)).ToList();

        var configTableStorageEntity = await _configTableStorage.GetFirst();
        var configs = JsonSerializer.Deserialize<Configs>(configTableStorageEntity.Configs);

        var (numberOfTeams, numberOfPlayers) = GetTeamsAndPlayersCount(selectedPlayerIds.Count, playersDTO, configs);

        var teams = _dealer.SortTeamsRandom(playersDTO, numberOfTeams, numberOfPlayers, getTeams.UsePosition, configs);

        return new OkObjectResult(teams);
    }

    private (int numberOfTeams, int numberOfPlayers) GetTeamsAndPlayersCount(int playerCount, List<PlayerDTO> playersDTO, Configs configs)
    {
        if (configs.NumberOfTeams > 0 && configs.NumberOfPlayers > 0)
        {
            return (configs.NumberOfTeams, configs.NumberOfPlayers);
        }

        return DetermineTeamsAndPlayersCount(playerCount, playersDTO);
    }

    private (int numberOfTeams, int numberOfPlayers) DetermineTeamsAndPlayersCount(int playerCount, List<PlayerDTO> playersDTO)
    {
        return playerCount switch
        {
            11 => (2, 6),
            12 when playersDTO.Exists(p => p.Position == 1) => (2, 6),
            12 => (3, 4),
            14 => (3, 5),
            15 => (3, 5),
            20 when !playersDTO.Any(p => p.Position == 1) => (2, CalculatePlayersPerTeam(playerCount, 2)),
            > 21 => (2, CalculatePlayersPerTeam(playerCount, 2)),
            _ => (2, 5)
        };
    }

    private int CalculatePlayersPerTeam(int playerCount, int teamCount)
    {
        return playerCount % teamCount == 0 ? playerCount / teamCount : playerCount / teamCount + 1;
    }
}