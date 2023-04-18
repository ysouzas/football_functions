
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using football_functions.DTOs.Request;
using football_functions.DTOs.Response;
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

    public async Task<PlayerTableStorageEntity> AddRank(AddRankDTO dto)
    {
        var playerEntity = GetById(dto.Id);

        var ranks = JsonSerializer.Deserialize<RankDTO[]>(playerEntity.Ranks);

        ranks = ranks.Append(dto.Rank).ToArray();

        playerEntity.Ranks = JsonSerializer.Serialize(ranks);

        var tableResult = await InsertOrMerge(playerEntity);

        var entity = GetById(dto.Id);

        return entity;
    }

    public async Task<IEnumerable<PlayerTableStorageEntity>> GetAll()
    {
        TableQuery<PlayerTableStorageEntity> query = new TableQuery<PlayerTableStorageEntity>();

        var playersEntity = _table.ExecuteQuery(query);

        return playersEntity;
    }

    public PlayerTableStorageEntity GetById(string id)
    {
        var rangeQuery = new TableQuery<PlayerTableStorageEntity>()
                                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));

        var playerEntity = _table.ExecuteQuery(rangeQuery).FirstOrDefault();

        return playerEntity;
    }

    public async Task<TableResult> InsertOrMerge(PlayerTableStorageEntity entity)
    {
        var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

        var tableResult = await _table.ExecuteAsync(insertOrMergeOperation);

        return tableResult;
    }

    public async Task<TableResult> UpdateScore(PlayerTableStorageEntity entity)
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(entity.Ranks);
        var score = ranks.GenerateScore();
        entity.Score = (double)score;
        entity.LastUpdateDate = DateTime.Now;

        var result = await InsertOrMerge(entity);

        return result;
    }
}

