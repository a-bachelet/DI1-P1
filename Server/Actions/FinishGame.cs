
using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;

namespace Server.Actions;

public sealed record FinishGameParams(int? GameId = null, Game? Game = null);

public class FinishGameValidator : AbstractValidator<FinishGameParams>
{
    public FinishGameValidator()
    {
        RuleFor(p => p.GameId).NotEmpty().When(p => p.Game is null);
        RuleFor(p => p.Game).NotEmpty().When(p => p.GameId is null);
    }
}

public class FinishGame : IAction<FinishGameParams, Result<Game>>
{
    public Task<Result<Game>> PerformAsync(FinishGameParams actionParams)
    {
        throw new NotImplementedException();
    }
}
