using System;
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

        var dateTime = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        for (int i = 1; i <= 6; i++)
        {
            var montAgoDate = dateTime.AddMonths((-1 * i));

            var ranks = rank.Where(r => r.DateOnlyGeneral() > montAgoDate).OrderBy(c => c.Date).ToList();
            var count = ranks.Count;

            if (count >= (4 * i))
                return Math.Round(ranks.Sum(r => r.Score) / count, 2);
        }

        return Math.Round(rank.Sum(r => r.Score) / rank.Length, 2);
    }
}
