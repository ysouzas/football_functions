using System.Text.Json;
using football_functions.DTOs.Request;
using football_functions.Extensions;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class Player
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public Player(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }

    [FunctionName("Player")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var player = req.Body.Deserialize<GetPlayerById>();

        var playersEntity = _playerTableStorage.GetById(player.Id);
        var playerWithRanksDTO = playersEntity.ToPlayerWithRanksDTO();

        return new OkObjectResult(playerWithRanksDTO);
    }
}