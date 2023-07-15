using System;
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

public class AddPlayer
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public AddPlayer(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }

    [FunctionName("AddPlayer")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var player = req.Body.Deserialize<AddPlayerDTO>();

        var entity = new PlayerTableStorageEntity
        (
            "FOOTBALL",
            Guid.NewGuid().ToString(),
            (double)player.Score,
            player.Name,
            "[]",
            0
            );

        var playersEntity = await _playerTableStorage.InsertOrReplace(entity);

        return new OkObjectResult(playersEntity.ToPlayerDTO());
    }
}