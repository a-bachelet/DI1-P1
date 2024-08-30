using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Hubs.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public record StartGameParams(int? GameId = null, Game? Game = null);

public class StartGameValidator : AbstractValidator<StartGameParams>
{
    public StartGameValidator()
    {
        RuleFor(p => p.GameId).NotEmpty().When(p => p.Game is null);
        RuleFor(p => p.Game).NotEmpty().When(p => p.GameId is null);
    }
}

public class StartGame(
    IGamesRepository gamesRepository,
    IAction<StartRoundParams, Result<Round>> startRoundAction,
    IMainHubService mainHubService,
    IGameHubService gameHubService
) : IAction<StartGameParams, Result<Game>>
{
    public async Task<Result<Game>> PerformAsync(StartGameParams actionParams)
    {
        var actionValidator = new StartGameValidator();
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (gameId, game) = actionParams;

        game ??= await gamesRepository.GetById(gameId!.Value);

        if (game is null)
        {
            Result.Fail($"Game with Id \"{gameId}\" not found.");
        }

        if (!game!.CanBeStarted())
        {
            return Result.Fail("Game cannot be started.");
        }

        game.Status = GameStatus.InProgress;

        await gamesRepository.SaveGame(game);

        var startRoundParams = new StartRoundParams(Game: game);
        var startRoundResult = await startRoundAction.PerformAsync(startRoundParams);

        if (startRoundResult.IsFailed)
        {
            return Result.Fail(startRoundResult.Errors);
        }

        await mainHubService.UpdateJoinableGamesList();
        await gameHubService.UpdateCurrentGame(gameId: gameId);

        return Result.Ok(game);
    }
}
