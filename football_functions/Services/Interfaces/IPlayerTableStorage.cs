﻿using System.Threading.Tasks;
using football_functions.DTOs.Request;
using football_functions.Models;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Services.Interfaces;

public interface IPlayerTableStorage : ITableStorage<PlayerTableStorageEntity>
{
    public Task<PlayerTableStorageEntity> AddRank(AddRankDTO dto);

    public Task<TableResult> UpdateScore(PlayerTableStorageEntity entity);

}

