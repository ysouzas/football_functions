using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using football_functions.Models;
using football_functions.Options;
using football_functions.Services.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace football_functions.Services;

public class ConfigTableStorage : IConfigTableStorage
{
    private readonly CloudTable _table;

    public ConfigTableStorage(IOptions<ConnectionStrings> connectionStringsOptions)
    {
        var connectionStrings = connectionStringsOptions?.Value ?? throw new ArgumentNullException(nameof(ConnectionStrings));

        var cloudStorageAccount = CloudStorageAccount.Parse(connectionStrings.StorageUrl);
        var tableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference("config");
    }

    public Task<IEnumerable<ConfigTableStorageEntity>> GetAll()
    {
        throw new NotImplementedException();
    }

    public ConfigTableStorageEntity GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<ConfigTableStorageEntity> GetFirst()
    {
        TableQuery<ConfigTableStorageEntity> query = new();

        var configs = _table.ExecuteQuery(query);

        return configs.First();
    }

    public Task<TableResult> InsertOrMerge(ConfigTableStorageEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<ConfigTableStorageEntity> InsertOrReplace(ConfigTableStorageEntity entity)
    {
        throw new NotImplementedException();
    }
}
