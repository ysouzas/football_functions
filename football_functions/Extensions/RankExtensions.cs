using System;
using System.Linq;
using System.Text.Json;
using football_functions.DTOs.Response;
using football_functions.Models;

namespace football_functions.Extensions;

public static class RankExtensions
{
    public static DateOnly DateOnlyGeneral(this RankDTO dto)
    {
        return new DateOnly(dto.Date.Year, dto.Date.Month, dto.Date.Day);
    }

    public static decimal GenerateScore(this PlayerTableStorageEntity entity)
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(entity.Ranks);

        if (ranks.Length == 0)
            return 0;

        var eightLastRanks = ranks.OrderByDescending(c => c.Date).Take(8).ToList();
        var count = eightLastRanks.Count;

        return Math.Round(eightLastRanks.Sum(r => r.Score) / count, 2);
    }
}
