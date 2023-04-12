using football_functions.DTOs;
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
        return new PlayerDTO(Name, RowKey, ((decimal)Score));
    }
}

