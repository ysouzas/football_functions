using football_functions.DTOs.Response;

namespace football_functions.DTOs;

public readonly record struct PlayerWithRanksDTO(string Name, string Id, decimal Score, RankDTO[] Ranks);
