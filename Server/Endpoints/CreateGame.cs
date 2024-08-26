using FluentResults;

using Server.Actions;
using Server.Actions.Contracts;
using Server.Endpoints.Contracts;
using Server.Models;
using Server.Persistence;

namespace Server.Endpoints;

public class CreateGame : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("games", Handler).WithTags("Games");
    }

    public static async Task<IResult> Handler(
        CreateGameParams actionParams,
        WssDbContext context,
        IAction<CreateGameParams, Result<Game>> createGameAction
    )
    {
        using var transaction = context.Database.BeginTransaction();

        var actionResult = await createGameAction.PerformAsync(actionParams);

        if (actionResult.IsFailed)
        {
            await transaction.RollbackAsync();
            return Results.BadRequest(new { Errors = actionResult.Errors.Select(e => e.Message) });
        }

        await transaction.CommitAsync();

        return Results.Created((string) null!, new { actionResult.Value.Id });
    }
}
