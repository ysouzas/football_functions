using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using football_functions.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var playersEntity = _playerTableStorage.GetAll();
            var playersDTO = playersEntity.Select(p => p.ToDTO()).OrderByDescending(p => p.Score).ToList();

            if (req.Method == HttpMethod.Post.ToString())
            {
                var ids = JsonSerializer.Deserialize<List<string>>(req.Body);
                playersDTO = playersDTO.Where(p => ids.Contains(p.Id)).ToList();
                var teams = _dealer.SortTeamsRandom(playersDTO, 3);
                return new OkObjectResult(teams);


            }

            return new OkObjectResult(playersDTO);
        }
    }
}

