using System.Threading.Tasks;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class Score
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public Score(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }

    [FunctionName("Score")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        TableResult result = null;

        foreach (var entity in playersEntity)
        {
            result = await _playerTableStorage.UpdateScore(entity);
        }

        return new OkObjectResult(result);
    }
}
