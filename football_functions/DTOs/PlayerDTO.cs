using football_functions.Models.Enums;

namespace football_functions.DTOs;

public readonly record struct PlayerDTO(string Name, string Id, decimal Score, int Position);
