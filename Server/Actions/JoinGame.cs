using FluentResults;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record JoinGameParams(
    string PlayerName,
    string CompanyName,
    int? GameId = null,
    Game? Game = null
) : CreatePlayerParams(PlayerName, CompanyName, GameId, Game);

public class JoinGameValidator : CreatePlayerValidator;

public class JoinGame(
    IGamesRepository gamesRepository,
    IPlayersRepository playersRepository,
    IAction<CreateCompanyParams, Result<Company>> createCompanyAction
) : CreatePlayer(gamesRepository, playersRepository, createCompanyAction);
