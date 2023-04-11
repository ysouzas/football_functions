using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using football_functions.DTOs;
using football_functions.Extensions;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Models;

public class PlayerTableStorageEntity : TableEntity
{

    public PlayerTableStorageEntity()
    {

    }
    public PlayerTableStorageEntity(string partitionKey, string rowKey, double score, string name, string ranks)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
        Score = score;
        Name = name;
        Ranks = ranks;
    }

    public double Score { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Ranks { get; set; } = string.Empty;

    public PlayerDTO ToDTO()
    {
        var rank = JsonSerializer.Deserialize<List<RankDTO>>(Ranks);
        var score = 0.0M;

        var dateTime = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        var twoMonthsAgoDate = dateTime.AddMonths(-2);

        var ranks = rank.Where(r => r.DateOnlyGeneral() >= twoMonthsAgoDate).OrderBy(c => c.Date).ToList();
        var count = ranks.Count;

        if (count >= 8)
            score = Math.Round(ranks.Sum(r => r.Score) / count, 2);

        ranks = rank.Where(r => r.DateOnlyGeneral().Year >= twoMonthsAgoDate.Year).OrderBy(c => c.Date).ToList();

        count = ranks.Count;



        if (count >= 10 && score == 0.0M)
            score = Math.Round(ranks.Sum(r => r.Score) / count, 2);

        if (score == 0.0M && rank.Count > 0)
            score = Math.Round(rank.Sum(r => r.Score) / rank.Count, 2);


        return new PlayerDTO(Name, RowKey, ((decimal)Score), rank.ToArray());
    }
}

