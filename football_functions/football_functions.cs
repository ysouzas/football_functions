using System.Linq;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions
{
    public class football_functions
    {
        private readonly IPlayerTableStorage _playerTableStorage;
        private readonly IDealer _dealer;

        public football_functions(IPlayerTableStorage playerTableStorage, IDealer dealer)
        {
            _playerTableStorage = playerTableStorage;
            _dealer = dealer;
        }

        [FunctionName("football_functions")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var playersEntity = _playerTableStorage.GetAll();
            var playersDTO = playersEntity.Select(p => p.ToDTO()).OrderByDescending(p => p.Score).ToList();

            return new OkObjectResult(playersDTO);
        }
    }
}

