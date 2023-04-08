using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using football_functions.Services.Interfaces;
using System.Linq;

namespace football_functions
{
    public class football_functions
    {
        private readonly IPlayerTableStorage _playerTableStorage;

        public football_functions(IPlayerTableStorage playerTableStorage)
        {
            _playerTableStorage = playerTableStorage;
        }

        [FunctionName("football_functions")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var playersEntity = _playerTableStorage.GetAll();
            var playersDTO = playersEntity.Select(p => p.ToDTO());

            return new OkObjectResult(playersDTO);
        }
    }
}

