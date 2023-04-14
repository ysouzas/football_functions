﻿using System;
using System.Linq;
using football_functions.DTOs.Response;

namespace football_functions.Extensions;

public static class RankExtensions
{
    public static DateOnly DateOnlyGeneral(this RankDTO dto)
    {
        return new DateOnly(dto.Date.Year, dto.Date.Month, dto.Date.Day);
    }

    public static decimal GenerateScore(this RankDTO[] rank)
    {
        if (rank.Length == 0)
            return 0;

        var dateTime = new DateOnly(DateTime.Now.Year, DateTime.Now.Month,17);

        var twoMonthsAgoDate = dateTime.AddMonths(-2);

        var ranks = rank.Where(r => r.DateOnlyGeneral() >= twoMonthsAgoDate).OrderBy(c => c.Date).ToList();
        var count = ranks.Count;

        if (count >= 8)
            return Math.Round(ranks.Sum(r => r.Score) / count, 2);

        ranks = rank.Where(r => r.DateOnlyGeneral().Year >= twoMonthsAgoDate.Year).OrderBy(c => c.Date).ToList();

        count = ranks.Count;

        if (count >= 10)
            return Math.Round(ranks.Sum(r => r.Score) / count, 2);

        return Math.Round(rank.Sum(r => r.Score) / rank.Length, 2);
    }
}
