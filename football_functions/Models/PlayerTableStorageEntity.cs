using System;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Models;

public class PlayerTableStorageEntity : TableEntity
{

    public PlayerTableStorageEntity()
    {

    }

    public PlayerTableStorageEntity(string partitionKey, string rowKey, double score, string name, string ranks, int position, string avoidSameTeam, string needToBeAtSameTeam, bool tShirtPBN)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
        Score = score;
        Name = name;
        Ranks = ranks;
        LastUpdateDate = DateTime.UtcNow;
        Position = position;
        AvoidSameTeam = avoidSameTeam;
        NeedToBeAtSameTeam = needToBeAtSameTeam;
        TShirtPBN = tShirtPBN;
    }

    public double Score { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public string Ranks { get; set; } = string.Empty;

    public int Position { get; set; }

    public DateTime LastUpdateDate { get; set; }

    public string AvoidSameTeam { get; set; }

    public string NeedToBeAtSameTeam { get; set; }

    public bool TShirtPBN { get; set; }

}

