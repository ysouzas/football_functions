
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs;
using football_functions.Extensions;
using football_functions.Models;
using football_functions.Options;
using football_functions.Services.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace football_functions.Services;

public class PlayerTableStorage : IPlayerTableStorage
{
    private readonly CloudTable _table;

    public PlayerTableStorage(IOptions<ConnectionStrings> connectionStringsOptions)
    {
        var connectionStrings = connectionStringsOptions?.Value ?? throw new ArgumentNullException(nameof(ConnectionStrings));

        var cloudStorageAccount = CloudStorageAccount.Parse(connectionStrings.StorageUrl);
        var tableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference("football");
    }

    public async Task<TableResult> AddRank(AddRankDTO dto)
    {
        var rangeQuery = new TableQuery<PlayerTableStorageEntity>()
                                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, dto.Id));

        var playerEntity = _table.ExecuteQuery(rangeQuery).FirstOrDefault();

        var ranks = JsonSerializer.Deserialize<RankDTO[]>(playerEntity.Ranks);
        ranks = ranks.Append(dto.rank).ToArray();

        playerEntity.Ranks = JsonSerializer.Serialize(ranks);

        var tableResult = await InsertOrMerge(playerEntity);

        return tableResult;
    }

    public async Task<IEnumerable<PlayerTableStorageEntity>> GetAll()
    {
        TableQuery<PlayerTableStorageEntity> query = new TableQuery<PlayerTableStorageEntity>();

        var playersEntity = _table.ExecuteQuery(query);

        foreach (var playerEntity in playersEntity)
        {
            var ranks = JsonSerializer.Deserialize<RankDTO[]>(playerEntity.Ranks);
            var score = ranks.GenerateScore();
            playerEntity.Score = (double)score;

            await InsertOrMerge(playerEntity);
        }
        return playersEntity;
    }

    public async Task<TableResult> InsertOrMerge(PlayerTableStorageEntity entity)
    {
        var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

        var tableResult = await _table.ExecuteAsync(insertOrMergeOperation);

        return tableResult;
    }
}

