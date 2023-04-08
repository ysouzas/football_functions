
using System.Collections.Generic;

namespace football_functions.DTOs;


public readonly record struct TeamDTO(double Score, List<PlayerDTO> Players);


