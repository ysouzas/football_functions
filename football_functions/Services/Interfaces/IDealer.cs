using System;
using System.Collections.Generic;
using System.Numerics;
using football_functions.DTOs;

namespace football_functions.Services.Interfaces
{
    public interface IDealer
    {
        public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams);
    }
}