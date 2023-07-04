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

        var dateTime = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        var oneMonthAgoDate = dateTime.AddMonths(-1);

        var oneMonthRanks = ranks.Where(r => r.DateOnlyGeneral() > oneMonthAgoDate).OrderBy(c => c.Date).ToList();
        var count = oneMonthRanks.Count;

        if (count >= 8)
            return Math.Round(oneMonthRanks.Sum(r => r.Score) / count, 2);

        var twoMonthsAgoDate = dateTime.AddMonths(-2);

        var twoMonthsRanks = ranks.Where(r => r.DateOnlyGeneral() > twoMonthsAgoDate).OrderBy(c => c.Date).ToList();
        count = twoMonthsRanks.Count;

        if (count >= 8)
            return Math.Round(twoMonthsRanks.Sum(r => r.Score) / count, 2);

        return Math.Round(ranks.Sum(r => r.Score) / ranks.Length, 2);
    }
}
