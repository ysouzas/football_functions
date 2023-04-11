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

namespace football_functions
{
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
            var playersEntity = _playerTableStorage.GetAll();
            var playersDTO = playersEntity.Select(p => p.ToDTO()).OrderByDescending(p => p.Score).ToList();

            var ids = JsonSerializer.Deserialize<List<string>>(req.Body);
            playersDTO = playersDTO.Where(p => ids.Contains(p.Id)).ToList();
            var teams = _dealer.SortTeamsRandom(playersDTO, 3);
            return new OkObjectResult(teams);

        }
    }
}
