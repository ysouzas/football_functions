﻿using System;
using System.Linq;
using System.Text.Json;
using football_functions.DTOs.Response;
using football_functions.Models;

namespace football_functions.Extensions;

public static class RankExtensions
{
    public static DateOnly ToDateOnly(this DateTime date)
    {
        return new DateOnly(date.Year, date.Month, date.Day);
    }

    public static decimal GenerateScore(this PlayerTableStorageEntity entity)
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(entity.Ranks);

        if (ranks.Length == 0)
            return 0;

        var tenLastRanks = ranks.OrderByDescending(c => c.Date).Take(8).ToList();

        return Math.Round(tenLastRanks.Sum(r => r.Score) / tenLastRanks.Count, 2);
    }
}