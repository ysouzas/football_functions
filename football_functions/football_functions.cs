using System.Linq;
using System.Threading.Tasks;
using football_functions.Extensions;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions;

public class football_functions
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public football_functions(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }

    [FunctionName("football_functions")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        var playersDTO = playersEntity.Select(p => p.ToPlayerDTO()).OrderByDescending(p => p.Score).ToList();

        return new OkObjectResult(playersDTO);
    }
}

