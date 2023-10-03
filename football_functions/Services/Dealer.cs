using System;
using System.Collections.Generic;
using System.Linq;
using football_functions.DTOs;
using football_functions.DTOs.Response;
using football_functions.Models.Enums;
using football_functions.Services.Interfaces;

namespace football_functions.Services;

public class Dealer : IDealer
{
    public TeamDTO[] SortTeamsRandom(IEnumerable<PlayerDTO> players, int numberOfTeams, int numberOfPlayers)
    {
        var inicialTeam = 0;
        var finalTeam = numberOfTeams == 2 ? 1 : 2;
        var numberOfLast = 2;

        var numberOfPossibilities = 100000;
        TeamDTO[] teams = Array.Empty<TeamDTO>();

        decimal bet = numberOfTeams == 2 ? 3.0M : 1.0M;
        var totalScore = players.Sum(p => p.Score);

        var acceptableDifference = (totalScore % 3) == 0 ? 0.0M : 0.01M;

        var numberOfSameTeam = players.Count(p => p.NeedToBeAtSameTeam is true);

        if (players.Count() < 12 || players.Count() == 14)
        {
            inicialTeam = 1;
            finalTeam = 2;
        }


        if (players.Count() == 15)
        {
            numberOfLast = 3;
        }


        if (players.Sum(p => p.Score) > 100)
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


            var hasOneWithAllSameTeam = randomTeams[inicialTeam].Count(p => p.NeedToBeAtSameTeam) == numberOfSameTeam;
            hasOneWithAllSameTeam = hasOneWithAllSameTeam ? hasOneWithAllSameTeam : randomTeams[finalTeam].Count(p => p.NeedToBeAtSameTeam) == numberOfSameTeam;

            var hasRandomTeam1MoreThanOneGoalkeeper = randomTeams[inicialTeam].Count(p => p.Position == (int)Position.Goalkeeper) > 1;
            var hasRandomTeam2MoreThanOneGoalkeeper = randomTeams[finalTeam].Count(p => p.Position == (int)Position.Goalkeeper) > 1;

            if (numberOfTeams > 2)
            {
                hasRandomTeam3MoreThanOneGoalkeeper = randomTeams[1].Count(p => p.Position == (int)Position.Goalkeeper) > 1;
                hasOneWithAllSameTeam = hasOneWithAllSameTeam ? hasOneWithAllSameTeam : randomTeams[1].Count(p => p.NeedToBeAtSameTeam) == numberOfSameTeam;
            }

            if (hasRandomTeam3MoreThanOneGoalkeeper || hasRandomTeam1MoreThanOneGoalkeeper || hasRandomTeam2MoreThanOneGoalkeeper ||
                !hasOneWithAllSameTeam)
                continue;

            var oneTeamHasMoreThanHalfPosition = false;

            if (numberOfTeams == 2 || players.Count() > 17)
            {
                foreach (Position value in Enum.GetValues(typeof(Position)))
                {
                    oneTeamHasMoreThanHalfPosition = HasMoreThanHalfPlayersOfPosition(randomTeams, players, value, inicialTeam, finalTeam);
                    if (oneTeamHasMoreThanHalfPosition) break;
                }
            }

            if (oneTeamHasMoreThanHalfPosition || !hasOneWithAllSameTeam)
                continue;

            var differenceFromTeam0 = randomTeams[inicialTeam].Sum(p => p.Score);
            var differenceFromTeam2 = randomTeams[finalTeam].Sum(p => p.Score);

            var differenceBetweenTeam2And0 = differenceFromTeam2 - differenceFromTeam0;

            if (differenceBetweenTeam2And0 < bet)
            {
                bet = differenceBetweenTeam2And0;
                teams = randomTeams.Select(asa => new TeamDTO(asa.Sum(p => p.Score), asa.OrderByDescending(a => a.Score).ToList())).OrderBy(t => t.Score).ToArray();
            }

            if (bet == acceptableDifference || bet == 0.00M)
            {
                return teams;
            }
        }

        return teams;
    }

    private bool HasMoreThanHalfPlayersOfPosition(PlayerDTO[][] randomTeams, IEnumerable<PlayerDTO> players, Position position, int inicialTeam, int finalTeam)
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