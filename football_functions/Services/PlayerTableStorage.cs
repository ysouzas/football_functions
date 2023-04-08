
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using football_functions.Models;
using football_functions.Options;
using football_functions.Services.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace football_functions;

public class PlayerTableStorage : IPlayerTableStorage
{
    private CloudTable _table;

    public PlayerTableStorage(IOptions<ConnectionStrings> connectionStringsOptions)
    {
        var connectionStrings = connectionStringsOptions?.Value ?? throw new ArgumentNullException(nameof(ConnectionStrings));

        var cloudStorageAccount = CloudStorageAccount.Parse(connectionStrings.StorageUrl);
        var tableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference("football");
    }

    public IEnumerable<PlayerTableStorageEntity> GetAll()
    {
        TableQuery<PlayerTableStorageEntity> query = new TableQuery<PlayerTableStorageEntity>();

        var tableResult = _table.ExecuteQuery(query);

        return tableResult;
    }
}

