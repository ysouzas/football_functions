namespace football_functions.DTOs.Response;

public readonly record struct PlayerDTO(string Name, string Id, decimal Score, int Position, string AvoidSameTeam, string NeedToBeAtSameTeam, bool TshirtPBN, bool TshirtGreen);
