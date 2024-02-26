namespace football_functions.DTOs.Request;

public record struct AddPlayerDTO(string Name, decimal Score = 0, bool TshirtPBN = false, bool TshirtGreen = false);
