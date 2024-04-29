using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Models;
public class ConfigTableStorageEntity : TableEntity
{

    public ConfigTableStorageEntity()
    {

    }

    public ConfigTableStorageEntity(string partitionKey, string rowKey, string configs)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
        Configs = configs;

    }

    public string Configs { get; set; } = string.Empty;
}