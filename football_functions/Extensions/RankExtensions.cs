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

        for (int i = 2; i <= 5; i++)
        {
            var j = i;
            var numberOfRanks = (4 * i);

            if (entity.Mensalista > 1)
            {
                j = i - 1;

                numberOfRanks = entity.Mensalista > j ? (4 * entity.Mensalista) : (4 * j);
            }

            var montAgoDate = dateTime.AddMonths(j * -1);

            var rank = ranks.Where(r => r.DateOnlyGeneral() > montAgoDate).OrderBy(c => c.Date).ToList();
            var count = rank.Count;

            if (count >= numberOfRanks)
                return Math.Round(rank.Sum(r => r.Score) / count, 2);
        }

        return Math.Round(ranks.Sum(r => r.Score) / ranks.Length, 2);
    }
}
