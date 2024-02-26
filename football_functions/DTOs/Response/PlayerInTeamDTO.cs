namespace football_functions.DTOs.Response;

public readonly record struct PlayerInTeamDTO(string Name, string Id, decimal Score, int Position, bool TshirtPBN, bool TshirtGreen);
