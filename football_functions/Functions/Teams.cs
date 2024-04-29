using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs.Request;
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
        var playersDTO = playersEntity.Select(p => p.ToPlayerDTO()).OrderByDescending(p => p.Score).ToList();

        var getTeams = JsonSerializer.Deserialize<GetTeams>(req.Body);

        var ids = getTeams.Ids;

        playersDTO = playersDTO.Where(p => ids.Contains(p.Id)).ToList();

        var numberOfTeams = 3;
        var numberOfPlayers = 5;

        if (ids.Count == 20 && !playersDTO.Any(p => p.Position == 1))
        {
            numberOfTeams = 2;

            numberOfPlayers = ids.Count % 2 == 0 ? ids.Count / 2 : ids.Count / 2 + 1;
        }
        else if (ids.Count > 21)
        {
            numberOfTeams = 2;

            numberOfPlayers = ids.Count % 2 == 0 ? ids.Count / 2 : ids.Count / 2 + 1;
        }
        else if (ids.Count > 15)
        {
            numberOfTeams = 3;

            numberOfPlayers = ids.Count % 3 == 0 ? ids.Count / 3 : ids.Count / 3 + 1;
        }
        else if (ids.Count == 11)
        {
            numberOfTeams = 2;

            numberOfPlayers = 6;
        }
        else if (ids.Count == 12 && playersDTO.Exists(p => p.Position == 1))
        {
            numberOfTeams = 2;

            numberOfPlayers = 6;
        }
        else if (ids.Count == 12)
        {
            numberOfTeams = 3;

            numberOfPlayers = 4;
        }
        else if (ids.Count == 14)
        {
            numberOfTeams = 3;

            numberOfPlayers = 5;
        }
        else if (ids.Count <= 10)
        {
            numberOfTeams = 2;

            numberOfPlayers = 5;
        }

        var configTableStorageEntity = await _configTableStorage.GetFirst();

        var configs = JsonSerializer.Deserialize<Configs>(configTableStorageEntity.Configs);


        var teams = _dealer.SortTeamsRandom(playersDTO, numberOfTeams, numberOfPlayers, getTeams.UsePosition, configs);
        return new OkObjectResult(teams);

    }
}
