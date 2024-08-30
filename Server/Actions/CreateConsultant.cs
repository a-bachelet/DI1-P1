
using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.SignalR;

using Server.Actions.Contracts;
using Server.Hubs;
using Server.Hubs.Contracts;
using Server.Models;
using Server.Persistence;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateConsultantParams(string ConsultantName, int? GameId = null, Game? Game = null);

public class CreateConsultantValidator : AbstractValidator<CreateConsultantParams>
{
    public CreateConsultantValidator()
    {
        RuleFor(p => p.ConsultantName).NotEmpty();
        RuleFor(p => p.GameId).NotEmpty().When(p => p.Game is null);
        RuleFor(p => p.Game).NotEmpty().When(p => p.GameId is null);
    }
}

public class CreateConsultant(
    IConsultantsRepository consultantsRepository,
    IGamesRepository gamesRepository,
    IGameHubService gameHubService
) : IAction<CreateConsultantParams, Result<Consultant>>
{
    public async Task<Result<Consultant>> PerformAsync(CreateConsultantParams actionParams)
    {
        var rnd = new Random();

        var actionValidator = new CreateConsultantValidator();
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (consultantName, gameId, game) = actionParams;

        game ??= await gamesRepository.GetById((int) gameId!);

        if (game is null)
        {
            return Result.Fail($"Game with Id \"{gameId}\" not found.");
        }

        IEnumerable<int> salaryRequirements = [];

        for (var salaryRequirement = 29000; salaryRequirement <= 100000; salaryRequirement += 500)
        {
            salaryRequirements = salaryRequirements.Append(salaryRequirement);
        }

        var randomSalaryRequirement = rnd.Next(salaryRequirements.Count() + 1);

        var consultant = new Consultant(consultantName, randomSalaryRequirement, (int) game!.Id!);

        await consultantsRepository.SaveConsultant(consultant);

        await gameHubService.UpdateCurrentGame(gameId: gameId);

        return Result.Ok(consultant);
    }
}
