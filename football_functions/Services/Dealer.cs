using System;
using System.Collections.Generic;
using System.Linq;
using football_functions.DTOs.Response;
using football_functions.Extensions;
using football_functions.Models;
using football_functions.Services.Interfaces;

namespace football_functions.Services;

public class Dealer : IDealer
{
    public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams, int numberOfPlayers, bool usePosition, Configs config)
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

        players = GetPlayersWithAvoid(players, players.OrderBy(p => p.Score).ToList(), config.AvoidWorstPlayersSameTeam, numberOfLast, "LAST");
        players = GetPlayersWithAvoid(players, players.OrderByDescending(p => p.Score).ToList(), config.AvoidBestPlayersSameTeam, numberOfLast, "FIRST");

        for (int i = 0; i < numberOfPossibilities; i++)
        {
            var r = new Random();


            var randomTeams = players.OrderBy(i => r.Next())
                                     .Chunk(numberOfPlayers)
                                     .OrderBy(p => p.Sum(p => p.Score))
                                     .ToArray();


            var allTags = players.SelectMany(p => p.AvoidSameTeam?.Split('.') ?? Enumerable.Empty<string>())
                                 .Where(s => !string.IsNullOrEmpty(s))
                                 .Distinct()
                                 .ToList();

            var hasMoreThanOneAvoidSameTeam = false;

            foreach (var tag in allTags)
            {
                foreach (var team in randomTeams)
                {
                    if (team.Count(p => !string.IsNullOrEmpty(p.AvoidSameTeam) && p.AvoidSameTeam.Contains(tag)) >= 2)
                    {
                        hasMoreThanOneAvoidSameTeam = true;
                        break;
                    }
                }
                if (hasMoreThanOneAvoidSameTeam) break;
            }

            if (hasMoreThanOneAvoidSameTeam)
                continue;


            var sameTeamTags = players.Where(p => !string.IsNullOrEmpty(p.NeedToBeAtSameTeam))
                                      .GroupBy(p => p.NeedToBeAtSameTeam)
                                      .ToDictionary(g => g.Key, g => g.Count());

            var dic = sameTeamTags.Keys.ToDictionary(k => k, _ => false);

            foreach (var sameTeamTag in sameTeamTags)
            {
                foreach (var team in randomTeams)
                {
                    if (team.Count(p => p.NeedToBeAtSameTeam == sameTeamTag.Key) == sameTeamTag.Value)
                    {
                        dic[sameTeamTag.Key] = true;
                    }
                }
            }

            if (dic.ContainsValue(false))
                continue;


            var hasRandomTeam1MoreThanOneGoalkeeper = randomTeams[inicialTeam].Count(p => p.Position == 1) > 1;
            var hasRandomTeam2MoreThanOneGoalkeeper = randomTeams[finalTeam].Count(p => p.Position == 1) > 1;
            var hasRandomTeam3MoreThanOneGoalkeeper = false;

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

                teams = randomTeams.Select(rt => new TeamDTO(rt.Sum(p => p.Score), rt.Select(p => p.ToPlayerInTeamDTO()).OrderByDescending(a => a.Score).ToList()))
                                   .ToArray();

                countBet = 0;
            }


            if (bet == acceptableDifference || bet == 0.00M || countBet == 15)
            {
                return OrderTeams(numberOfTeams, teams);
            }
        }

        return OrderTeams(numberOfTeams, teams);
    }

    private static IEnumerable<PlayerDTO> GetPlayersWithAvoid(IEnumerable<PlayerDTO> players, List<PlayerDTO> updatedPlayersDTO, bool config, int numberOfLast, string tag)
    {
        if (config)
        {
            for (int i = 0; i < numberOfLast; i++)
            {
                var player = updatedPlayersDTO[i];

                player = player with { AvoidSameTeam = string.IsNullOrEmpty(player.AvoidSameTeam) ? tag : player.AvoidSameTeam + $".{tag}" };
                updatedPlayersDTO[i] = player;
            }

            var existingIds = new HashSet<string>(updatedPlayersDTO.Select(p => p.Id));
            var newPlayers = players.Where(player => !existingIds.Contains(player.Id)).ToList();
            updatedPlayersDTO.AddRange(newPlayers);

            players = updatedPlayersDTO;
        }

        return players;
    }

    private static TeamDTO[] OrderTeams(int numberOfTeams, TeamDTO[] teams)
    {
        var orderedByGreen = teams.OrderByDescending(t => t.Players.Count(p => p.TshirtGreen)).ToList();

        var teamWithMostGreen = orderedByGreen.First();
        orderedByGreen.RemoveAt(0);

        var orderedByPBN = orderedByGreen.OrderByDescending(t => t.Players.Count(p => p.TshirtPBN)).ToList();

        var teamWithMostPBN = orderedByPBN.First();
        orderedByPBN.RemoveAt(0);

        var teamWithMostBlack = orderedByPBN.First();

        var teamsByTshirt = new List<TeamDTO> { teamWithMostGreen, teamWithMostPBN, teamWithMostBlack };

        if (numberOfTeams == 2)
        {
            teamsByTshirt.RemoveAt(2);
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