using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateGameParams(string GameName, string PlayerName, string CompanyName, int Rounds = 15);

public class CreateGameValidator : AbstractValidator<CreateGameParams>
{
    public CreateGameValidator(IGamesRepository gamesRepository)
    {
        RuleFor(actionParams => actionParams.GameName)
          .NotNull()
          .NotEmpty()
          .MustAsync(async (gameName, _token) => await gamesRepository.IsGameNameAvailable(gameName))
          .WithMessage("'Game Name' is already in use.");

        RuleFor(actionParams => actionParams.PlayerName)
          .NotNull()
          .NotEmpty();

        RuleFor(actionParams => actionParams.CompanyName)
          .NotNull()
          .NotEmpty();

        RuleFor(actionParams => actionParams.Rounds)
          .NotNull()
          .GreaterThanOrEqualTo(15);
    }
}

public class CreateGame(
  ICompaniesRepository companiesRepository,
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : IAction<CreateGameParams, Result<Game>>
{
    public async Task<Result<Game>> PerformAsync(CreateGameParams actionParams)
    {
        var actionValidator = new CreateGameValidator(gamesRepository);
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (gameName, playerName, companyName, rounds) = actionParams;

        var game = new Game(gameName, rounds);

        await gamesRepository.SaveGame(game);

        var createPlayer = new CreatePlayer(companiesRepository, gamesRepository, playersRepository);
        var createPlayerParams = new CreatePlayerParams(playerName, companyName, game.Id!.Value);
        var createPlayerResult = await createPlayer.PerformAsync(createPlayerParams);

        if (createPlayerResult.IsFailed)
        {
            return Result.Fail(createPlayerResult.Errors);
        }

        return Result.Ok(game);
    }
}
