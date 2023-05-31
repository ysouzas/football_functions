using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

    public Teams(IPlayerTableStorage playerTableStorage, IDealer dealer)
    {
        _playerTableStorage = playerTableStorage;
        _dealer = dealer;
    }

    [FunctionName("Teams")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        var playersDTO = playersEntity.Select(p => p.ToPlayerDTO()).OrderByDescending(p => p.Score).ToList();

        var ids = JsonSerializer.Deserialize<List<string>>(req.Body);
        playersDTO = playersDTO.Where(p => ids.Contains(p.Id)).ToList();

        var numberOfTeams = 3;
        var numberOfPlayers = 5;
        if (ids.Count > 21)
        {
            numberOfTeams = 2;

            numberOfPlayers = ids.Count % 2 == 0 ? ids.Count / 2 : ids.Count / 2 + 1;

        }
        else if (ids.Count > 15)
        {
            numberOfTeams = 3;

            numberOfPlayers = ids.Count % 3 == 0 ? ids.Count / 3 : ids.Count / 3 + 1;
        }
        else if (ids.Count <= 12)
        {
            numberOfTeams = 2;

            numberOfPlayers = 6;
        }

        var teams = _dealer.SortTeamsRandom(playersDTO, numberOfTeams, numberOfPlayers);
        return new OkObjectResult(teams);

    }
}
