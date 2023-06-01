using System;
using System.Linq;
using System.Threading.Tasks;
using football_functions.Extensions;
using football_functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class Ranking
{
    private readonly IPlayerTableStorage _playerTableStorage;
    private readonly IDealer _dealer;

    public Ranking(IPlayerTableStorage playerTableStorage, IDealer dealer)
    {
        _playerTableStorage = playerTableStorage;
        _dealer = dealer;
    }

    [FunctionName("Ranking")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var dateTime = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        var twoMonthAgoDate = dateTime.AddMonths(-1);
        var playersEntity = await _playerTableStorage.GetAll();
        var playersDTO = playersEntity.Select(p => p.ToPlayerWithRanksDTO()).Where(p => p.Ranks.Any(r => r.DateOnlyGeneral() >= twoMonthAgoDate)).OrderByDescending(p => p.Score).ToList();

        var text = $"Ranking Geral - {twoMonthAgoDate:dd/MM/yyyy} - {dateTime:dd/MM/yyyy}\n";


        for (int i = 0; i < playersDTO.Count; i++)
        {
            var player = playersDTO[i];

            text += $"{i + 1} - {player.Name} - {player.Score:0.00}\n";
        }

        return new JsonResult(text);

    }
}
