using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public record CreatePlayerParams(string PlayerName, string CompanyName, int GameId);

public class CreatePlayerValidator : AbstractValidator<CreatePlayerParams>
{
    public CreatePlayerValidator(
      IGamesRepository gamesRepository,
      IPlayersRepository playersRepository
    )
    {
        RuleFor(actionParams => actionParams.PlayerName)
          .NotNull()
          .NotEmpty();

        RuleFor(actionParams => actionParams.CompanyName)
          .NotNull()
          .NotEmpty();

        RuleFor(actionParams => actionParams.GameId)
          .NotNull()
          .NotEmpty()
          .MustAsync(async (gameId, _token) => await gamesRepository.GameExists(gameId))
          .WithMessage(actionParams => $"Game with Id \"{actionParams.GameId}\" not found.");

        RuleFor(actionParams => new { actionParams.PlayerName, actionParams.GameId })
          .MustAsync(async (actionParams, _token) =>
            await playersRepository.IsPlayerNameAvailable(actionParams.PlayerName, actionParams.GameId))
          .WithMessage("'Player Name' is already in use.");
    }
}

public class CreatePlayer(
  ICompaniesRepository companiesRepository,
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : IAction<CreatePlayerParams, Result<Player>>
{
    public async Task<Result<Player>> PerformAsync(CreatePlayerParams actionParams)
    {
        var actionValidator = new CreatePlayerValidator(gamesRepository, playersRepository);
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (playerName, companyName, gameId) = actionParams;

        var game = await gamesRepository.GetById(gameId);

        if (!game!.CanBeJoined())
        {
            return Result.Fail("Game is full and cannot be joined.");
        }

        var player = new Player(playerName, gameId);

        await playersRepository.SavePlayer(player);

        var createCompany = new CreateCompany(companiesRepository, gamesRepository, playersRepository);
        var createCompanyParams = new CreateCompanyParams(companyName, player.Id!.Value);
        var createCompanyResult = await createCompany.PerformAsync(createCompanyParams);

        if (createCompanyResult.IsFailed)
        {
            return Result.Fail(createCompanyResult.Errors);
        }

        return Result.Ok(player);
    }
}
