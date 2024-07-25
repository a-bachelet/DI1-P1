
using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
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
    IConsultantsRepository consultantsRepository
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

        return Result.Ok(consultant);
    }
}
