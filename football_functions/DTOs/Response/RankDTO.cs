using System;

namespace football_functions.DTOs.Response;

public record struct RankDTO(decimal Score, DayOfWeek DayOfWeek, DateTime Date);

