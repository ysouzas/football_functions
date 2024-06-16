namespace football_functions.Models;

public readonly record struct Configs(bool AvoidWorstPlayersSameTeam, bool AvoidBestPlayersSameTeam, int NumberOfPlayers = 0, int NumberOfTeams = 0);
