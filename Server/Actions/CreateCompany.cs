using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateCompanyParams(string CompanyName, int PlayerId);

public class CreateCompanyValidator : AbstractValidator<CreateCompanyParams>
{
    public CreateCompanyValidator(
      ICompaniesRepository companiesRepository,
      IGamesRepository gamesRepository,
      IPlayersRepository playersRepository
    )
    {
        RuleFor(actionParams => actionParams.CompanyName)
          .NotNull()
          .NotEmpty();

        RuleFor(actionParams => actionParams.PlayerId)
          .NotNull()
          .NotEmpty()
          .MustAsync(async (playerId, _token) => await playersRepository.PlayerExists(playerId))
          .WithMessage(actionParams => $"Player with Id \"{actionParams.PlayerId}\" not found.");

        RuleFor(actionParams => new { actionParams.CompanyName, actionParams.PlayerId })
          .MustAsync(async (actionParams, _token) =>
          {
              var game = await gamesRepository.GetByPlayerId(actionParams.PlayerId);

              if (game is null)
              {
                  return false;
              }

              return await companiesRepository.IsCompanyNameAvailable(actionParams.CompanyName, game.Id!.Value);
          })
          .WithMessage("'Company Name' is already in use.");
    }
}

public class CreateCompany(
  ICompaniesRepository companiesRepository,
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : IAction<CreateCompanyParams, Result<Company>>
{
    public async Task<Result<Company>> PerformAsync(CreateCompanyParams actionParams)
    {
        var actionValidator = new CreateCompanyValidator(companiesRepository, gamesRepository, playersRepository);
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (companyName, playerId) = actionParams;

        var player = await playersRepository.GetById(playerId);

        if (player!.CompanyId is not null)
        {
            return Result.Fail("Player already has a company.");
        }

        var company = new Company(companyName, playerId);

        await companiesRepository.SaveCompany(company);

        return Result.Ok(company);
    }
}
