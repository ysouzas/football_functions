﻿using System;
using System.Linq;
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

    public PlayerTableStorageEntity(string partitionKey, string rowKey, double score, string name, string ranks, bool goalkeeper = false)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
        Score = score;
        Name = name;
        Ranks = ranks;
        LastUpdateDate = DateTime.UtcNow;
        Goalkeeper = goalkeeper;
    }

    public double Score { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public string Ranks { get; set; } = string.Empty;

    public bool Goalkeeper { get; set; } = false;

    public DateTime LastUpdateDate { get; set; }

    public PlayerDTO ToPlayerDTO()
    {
        return new PlayerDTO(Name, RowKey, ((decimal)Score), Goalkeeper);
    }

    public PlayerWithRanksDTO ToPlayerWithRanksDTO()
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(Ranks);

        ranks = ranks.Length > 0 ? ranks.OrderByDescending(r => r.Date).ToArray() : Array.Empty<RankDTO>();
        return new PlayerWithRanksDTO(Name, RowKey, (decimal)Score, ranks);
    }
}

