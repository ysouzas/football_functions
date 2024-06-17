using System;
using System.Linq;
using System.Text.Json;
using football_functions.DTOs;
using football_functions.DTOs.Response;
using football_functions.Models;

namespace football_functions.Extensions;

public static class MappingExtentions
{
    public static PlayerDTO ToPlayerDTO(this PlayerTableStorageEntity me)
    {
        return new PlayerDTO(me.Name, me.RowKey, ((decimal)me.Score), me.Position, me.AvoidSameTeam, me.NeedToBeAtSameTeam, me.TshirtPBN, me.TshirtGreen, me.TshirtBlack);
    }

    public static PlayerWithRanksDTO ToPlayerWithRanksDTO(this PlayerTableStorageEntity me)
    {
        var ranks = JsonSerializer.Deserialize<RankDTO[]>(me.Ranks);

        ranks = ranks.Length > 0 ? ranks.OrderByDescending(r => r.Date).ToArray() : Array.Empty<RankDTO>();

        return new PlayerWithRanksDTO(me.Name, me.RowKey, (decimal)me.Score, ranks);
    }

    public static PlayerInTeamDTO ToPlayerInTeamDTO(this PlayerDTO me)
    {
        return new PlayerInTeamDTO(me.Name, me.Id, (decimal)me.Score, me.Position, me.TshirtPBN, me.TshirtGreen, me.TshirtBlack);
    }

    public static PlayerPositionDTO ToPlayerPositionDTO(this PlayerTableStorageEntity me)
    {
        return new PlayerPositionDTO(me.Name, me.RowKey, me.Position);
    }
}
