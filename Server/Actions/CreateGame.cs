using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.SignalR;

using Server.Actions.Contracts;
using Server.Hubs;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateGameParams(string GameName, string PlayerName, string CompanyName, int Rounds = 15);

public class CreateGameValidator : AbstractValidator<CreateGameParams>
{
    public CreateGameValidator()
    {
        RuleFor(p => p.GameName).NotEmpty();
        RuleFor(p => p.PlayerName).NotEmpty();
        RuleFor(p => p.CompanyName).NotEmpty();
        RuleFor(p => p.Rounds).NotNull().GreaterThanOrEqualTo(15);
    }
}

public class CreateGame(
  IGamesRepository gamesRepository,
  IAction<CreatePlayerParams, Result<Player>> createPlayerAction,
  IHubContext<GameHub> hubContext
) : IAction<CreateGameParams, Result<Game>>
{
    public async Task<Result<Game>> PerformAsync(CreateGameParams actionParams)
    {
        var actionValidator = new CreateGameValidator();
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (gameName, playerName, companyName, rounds) = actionParams;

        var isGameNameAvailable = await gamesRepository.IsGameNameAvailable(gameName);

        if (!isGameNameAvailable)
        {
            return Result.Fail("'Game Name' is already in use.");
        }

        var game = new Game(gameName, rounds);

        await gamesRepository.SaveGame(game);

        var createPlayerParams = new CreatePlayerParams(playerName, companyName, Game: game);
        var createPlayerResult = await createPlayerAction.PerformAsync(createPlayerParams);

        if (createPlayerResult.IsFailed)
        {
            return Result.Fail(createPlayerResult.Errors);
        }

        await hubContext.Clients.Group(game.Name).SendAsync("GameCreated");

        return Result.Ok(game);
    }
}
