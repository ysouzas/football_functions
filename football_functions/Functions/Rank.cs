using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs.Request;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions
{
    public class Rank
    {
        private readonly IPlayerTableStorage _playerTableStorage;

        public Rank(IPlayerTableStorage playerTableStorage)
        {
            _playerTableStorage = playerTableStorage;
        }

        [FunctionName("Rank")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var rankToAdd = JsonSerializer.Deserialize<AddRankDTO>(req.Body, options);

            var response = await _playerTableStorage.AddRank(rankToAdd);

            return new OkObjectResult(response.Result);
        }
    }
}
