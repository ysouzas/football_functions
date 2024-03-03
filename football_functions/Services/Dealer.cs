using System;
using System.Collections.Generic;
using System.Linq;
using football_functions.DTOs.Response;
using football_functions.Extensions;
using football_functions.Services.Interfaces;

namespace football_functions.Services;

public class Dealer : IDealer
{
    public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams, int numberOfPlayers, bool usePosition)
    {
        var inicialTeam = 0;
        var finalTeam = numberOfTeams == 2 ? 1 : 2;

        var numberOfPossibilities = 1000000;
        TeamDTO[] teams = Array.Empty<TeamDTO>();

        decimal bet = numberOfTeams == 2 ? 20.0M : 10.0M;

        if (players.Count() < 14)
            bet = 15.0M;

        var totalScore = players.Sum(p => p.Score);

        var numberOfLast = numberOfTeams;

        var acceptableDifference = (totalScore % 3) == 0 ? 0.0M : 0.01M;


        var countBet = 0;


        if (false)
        {
            var updatedPlayersDTO = players.OrderBy(p => p.Score).ToList();

            for (int i = 0; i < numberOfLast; i++)
            {
                var player = updatedPlayersDTO[i];

                if (string.IsNullOrEmpty(player.AvoidSameTeam))
                {
                    player = player with { AvoidSameTeam = "Last" };
                }
                else
                {
                    var avoid = player.AvoidSameTeam + ".Last";
                    player = player with { AvoidSameTeam = avoid };
                }
                updatedPlayersDTO[i] = player;
            }

            updatedPlayersDTO = updatedPlayersDTO.OrderByDescending(p => p.Score).ToList();

            for (int i = 0; i < numberOfLast; i++)
            {
                var player = updatedPlayersDTO[i];

                if (string.IsNullOrEmpty(player.AvoidSameTeam))
                {
                    player = player with { AvoidSameTeam = ".FIRST" };
                }
                else
                {
                    var avoid = player.AvoidSameTeam + ".FIRST";
                    player = player with { AvoidSameTeam = avoid };
                }
                updatedPlayersDTO[i] = player;
            }

            var newPlayes = new List<PlayerDTO>();

            foreach (var player in players)
            {
                var hasNOT = !updatedPlayersDTO.Where(p => p.Id == player.Id).Any();

                if (hasNOT)
                    updatedPlayersDTO.Add(player);
            }

            players = updatedPlayersDTO;
        }

        for (int i = 0; i < numberOfPossibilities; i++)
        {
            var r = new Random();

            var randomTeams = players.OrderBy(i => r.Next()).Chunk(numberOfPlayers).OrderBy(p => p.Sum(p => p.Score)).ToArray();

            var hasRandomTeam3MoreThanOneGoalkeeper = false;


            var allTags = players.Select(p => p.AvoidSameTeam).Where(s => !string.IsNullOrEmpty(s)).SelectMany(s => s.Split(".")).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
            var hasMoreThanOneAvoidSameTeam = false;

            foreach (var tag in allTags)
            {
                foreach (var team in randomTeams)
                {
                    hasMoreThanOneAvoidSameTeam = team.Where(p => !string.IsNullOrEmpty(p.AvoidSameTeam) && p.AvoidSameTeam.Contains(tag)).Count() >= 2;

                    if (hasMoreThanOneAvoidSameTeam) break;
                }
                if (hasMoreThanOneAvoidSameTeam) break;
            }

            if (hasMoreThanOneAvoidSameTeam)
                continue;

            var sameTeamTags = players.Where(p => !string.IsNullOrEmpty(p.NeedToBeAtSameTeam)).Select(p => p.NeedToBeAtSameTeam).GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            var dic = players.Where(p => !string.IsNullOrEmpty(p.NeedToBeAtSameTeam)).Select(p => p.NeedToBeAtSameTeam).GroupBy(x => x).ToDictionary(g => g.Key, g => false);

            foreach (var sameTeamTag in sameTeamTags)
            {
                foreach (var team in randomTeams)
                {
                    hasMoreThanOneAvoidSameTeam = team.Count(p => p.NeedToBeAtSameTeam == sameTeamTag.Key) == sameTeamTag.Value;

                    if (hasMoreThanOneAvoidSameTeam)
                    {
                        dic[sameTeamTag.Key] = true;
                    };

                }
            }

            if (dic.ContainsValue(false))
                continue;

            var hasRandomTeam1MoreThanOneGoalkeeper = randomTeams[inicialTeam].Count(p => p.Position == 1) > 1;
            var hasRandomTeam2MoreThanOneGoalkeeper = randomTeams[finalTeam].Count(p => p.Position == 1) > 1;

            if (numberOfTeams > 2)
            {
                hasRandomTeam3MoreThanOneGoalkeeper = randomTeams[1].Count(p => p.Position == 1) > 1;
            }

            if (hasRandomTeam3MoreThanOneGoalkeeper || hasRandomTeam1MoreThanOneGoalkeeper || hasRandomTeam2MoreThanOneGoalkeeper)
                continue;

            var oneTeamHasMoreThanHalfPosition = false;

            if (usePosition)
            {
                var positions = players.Select(p => p.Position).Distinct();

                foreach (var value in positions)
                {
                    oneTeamHasMoreThanHalfPosition = HasMoreThanHalfPlayersOfPosition(randomTeams, players, value, inicialTeam, finalTeam);
                    if (oneTeamHasMoreThanHalfPosition) break;
                }
            }

            if (oneTeamHasMoreThanHalfPosition)
                continue;

            var differenceFromTeam0 = randomTeams[inicialTeam].Sum(p => p.Score);
            var differenceFromTeam2 = randomTeams[finalTeam].Sum(p => p.Score);

            var differenceBetweenTeam2And0 = differenceFromTeam2 - differenceFromTeam0;


            if (differenceBetweenTeam2And0 == bet)
            {
                countBet++;
            }

            if (differenceBetweenTeam2And0 < bet)
            {
                bet = differenceBetweenTeam2And0;

                if (players.Count() % numberOfPlayers == 0)
                {
                    teams = randomTeams.Select(rt => new TeamDTO(rt.Sum(p => p.Score), rt.Select(rt => rt.ToPlayerInTeamDTO())
                                       .OrderByDescending(a => a.Score).ToList()))
                                       .ToArray();
                }
                else
                {
                    teams = randomTeams.Select(rt => new TeamDTO(rt.Sum(p => p.Score), rt.Select(rt => rt.ToPlayerInTeamDTO())
                                       .OrderByDescending(a => a.Score).ToList()))
                                       .ToArray();
                }

                countBet = 0;
            }

            if (bet == acceptableDifference || bet == 0.00M || countBet == 15)
            {
                return OrderTeams(numberOfTeams, teams);
            }
        }

        return OrderTeams(numberOfTeams, teams);


    }

    private static TeamDTO[] OrderTeams(int numberOfTeams, TeamDTO[] teams)
    {
        var orderedTeams = teams.OrderByDescending(t => t.Players.Count(p => p.TshirtPBN == true)).ToList();

        var teamsByTshirt = new List<TeamDTO>();

        if (numberOfTeams == 2)
        {
            teamsByTshirt.Add(orderedTeams[1]);
            teamsByTshirt.Add(orderedTeams[0]);
        }
        else
        {
            var TshirPBN = orderedTeams[0];
            orderedTeams.RemoveAt(0);
            orderedTeams = orderedTeams.OrderByDescending(t => t.Players.Count(p => p.TshirtGreen == true)).ToList();
            var TshirGreen = orderedTeams.First();
            orderedTeams.RemoveAt(0);

            teamsByTshirt.Add(TshirGreen);
            teamsByTshirt.Add(TshirPBN);
            teamsByTshirt.Add(orderedTeams[0]);

        }

        return teamsByTshirt.ToArray();
    }

    private bool HasMoreThanHalfPlayersOfPosition(PlayerDTO[][] randomTeams, IEnumerable<PlayerDTO> players, int position, int inicialTeam, int finalTeam)
    {
        var accptableNumber = Math.Ceiling(players.Count(p => p.Position == (int)position) * 0.5);

        var oneTeamHasMoreThanHalfPosition = randomTeams[inicialTeam].Count(p => p.Position == (int)position) > accptableNumber;

        if (finalTeam == 2)
        {
            oneTeamHasMoreThanHalfPosition = randomTeams[1].Count(p => p.Position == (int)position) > accptableNumber;
        }

        return oneTeamHasMoreThanHalfPosition ?
                                          oneTeamHasMoreThanHalfPosition :
                                          randomTeams[finalTeam].Count(p => p.Position == (int)position) > accptableNumber;
    }
}