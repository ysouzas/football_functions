using System;
using System.Collections.Generic;
using System.Linq;
using football_functions.DTOs;
using football_functions.Extensions;
using football_functions.Services.Interfaces;

namespace football_functions.Services
{
    public class Dealer : IDealer
    {
        public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams)
        {
            var numberOfPossibilities = 1000000;
            TeamDTO[] teams = Array.Empty<TeamDTO>();

            double bet = 0.10;
            var totalScore = players.Sum(p => p.Score);

            var acceptableDifference = (totalScore % 3) == 0 ? 0.0 : 0.01;

            for (int i = 0; i < numberOfPossibilities; i++)
            {
                var r = new Random();

                var randomTeams = players.OrderBy(i => r.Next()).Chunk(5).OrderBy(p => p.Sum(p => p.Score)).ToArray();
                var differenceFromTeam0 = randomTeams[0].Sum(p => p.Score);
                var differenceFromTeam1 = randomTeams[1].Sum(p => p.Score);
                var differenceFromTeam2 = randomTeams[2].Sum(p => p.Score);

                var differenceBetweenTeam2And0 = differenceFromTeam2 - differenceFromTeam0;

                if (differenceBetweenTeam2And0 < bet)
                {
                    bet = differenceBetweenTeam2And0;
                    teams = randomTeams.Select(asa => new TeamDTO(asa.Sum(p => p.Score), asa.ToList())).OrderBy(t => t.Score).ToArray();
                }

                if (bet == acceptableDifference || bet == 0.00)
                {
                    return teams;
                }
            }

            return teams;
        }
    }
}