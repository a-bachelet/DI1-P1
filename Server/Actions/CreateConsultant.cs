
using FluentResults;

using FluentValidation;

using Microsoft.AspNetCore.SignalR;

using Server.Actions.Contracts;
using Server.Hubs;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateConsultantParams(string ConsultantName);

public class CreateConsultantValidator : AbstractValidator<CreateConsultantParams>
{
    public CreateConsultantValidator()
    {
        RuleFor(p => p.ConsultantName).NotEmpty();
    }
}

public class CreateConsultant(
    IConsultantsRepository consultantsRepository,
    IHubContext<GameHub> hubContext
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

        IEnumerable<int> salaryRequirements = [];

        for (var salaryRequirement = 29000; salaryRequirement <= 100000; salaryRequirement += 500)
        {
            salaryRequirements = salaryRequirements.Append(salaryRequirement);
        }

        var randomSalaryRequirement = rnd.Next(salaryRequirements.Count() + 1);

        var consultant = new Consultant(actionParams.ConsultantName, randomSalaryRequirement);

        await consultantsRepository.SaveConsultant(consultant);

        await hubContext.Clients.Group("@TODO LINK CONSULTANT TO GAME").SendAsync("ConsultantCreated", consultant.Name);

        return Result.Ok(consultant);
    }
}
