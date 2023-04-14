
using System.Collections.Generic;

namespace football_functions.DTOs.Response;


public readonly record struct TeamDTO(decimal Score, List<PlayerDTO> Players);


