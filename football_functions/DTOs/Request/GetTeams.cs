using System.Collections.Generic;

namespace football_functions.DTOs.Request;

public record struct GetTeams(List<string> Ids, bool UsePosition = false);
