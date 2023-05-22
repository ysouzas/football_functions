using System;
using System.Threading.Tasks;
using football_functions.Services.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace football_functions.Functions;

public class UpdateScore
{
    private readonly IPlayerTableStorage _playerTableStorage;

    public UpdateScore(IPlayerTableStorage playerTableStorage)
    {
        _playerTableStorage = playerTableStorage;
    }


    [FunctionName("UpdateScore")]
    public async Task Run([TimerTrigger("0 0 6 * * 1,3")] TimerInfo myTimer, ILogger log)
    {
        var playersEntity = await _playerTableStorage.GetAll();
        TableResult result = null;

        foreach (var entity in playersEntity)
        {
            result = await _playerTableStorage.UpdateScore(entity);

        }

        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

    }
}
