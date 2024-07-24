using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public record StartGameParams(int GameId);

public class StartGameValidator : AbstractValidator<StartGameParams>
{
    public StartGameValidator(IGamesRepository gamesRepository)
    {
        RuleFor(actionParams => actionParams.GameId)
          .NotNull()
          .NotEmpty()
          .MustAsync(async (gameId, _token) => await gamesRepository.GameExists(gameId))
          .WithMessage(actionParams => $"Game with Id \"{actionParams.GameId}\" not found.");
    }
}

public class StartGame(IGamesRepository gamesRepository) : IAction<StartGameParams, Result<Game>>
{
    public async Task<Result<Game>> PerformAsync(StartGameParams actionParams)
    {
        var actionValidator = new StartGameValidator(gamesRepository);
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var game = await gamesRepository.GetById(actionParams.GameId);

        if (!game!.CanBeStarted())
        {
            return Result.Fail("Game cannot be started.");
        }

        game.Status = GameStatus.InProgress;

        await gamesRepository.SaveGame(game);

        return Result.Ok(game);
    }
}
