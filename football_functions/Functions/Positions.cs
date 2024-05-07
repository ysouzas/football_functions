using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs.Request;
using football_functions.Extensions;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class Positions
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public Positions(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }

    [FunctionName("Positions")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        var playerPositionDTO = playersEntity.Select(p => p.ToPlayerPositionDTO()).OrderBy(p => p.Position).ToList();

        var getPlayersPosition = JsonSerializer.Deserialize<GetPosition>(req.Body);

        var ids = getPlayersPosition.Ids;

        playerPositionDTO = playerPositionDTO.Where(p => ids.Contains(p.Id)).ToList();


        return new OkObjectResult(playerPositionDTO);

    }
}