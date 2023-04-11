using System;
using football_functions.DTOs;

namespace football_functions.Extensions
{
    public static class RankExtensions
    {
        public static DateOnly DateOnlyGeneral(this RankDTO dto)
        {

            return new DateOnly(dto.Date.Year, dto.Date.Month, dto.Date.Day);

        }
    }
}
