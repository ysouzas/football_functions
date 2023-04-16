using System;
using System.Text.Json;
using football_functions.DTOs;
using football_functions.DTOs.Response;
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

    public DateTime LastUpdateDate { get; set; }

    public PlayerDTO ToPlayerDTO()
    {
        return new PlayerDTO(Name, RowKey, ((decimal)Score));
    }

    public PlayerWithRanksDTO ToPlayerWithRanksDTO()
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(Ranks);

        return new PlayerWithRanksDTO(Name, RowKey, (decimal)Score, ranks);
    }
}

