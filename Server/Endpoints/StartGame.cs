using FluentResults;

using Server.Actions;
using Server.Actions.Contracts;
using Server.Endpoints.Contracts;
using Server.Models;
using Server.Persistence;

namespace Server.Endpoints;

public class StartGame : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("games/{gameId}/start", Handler).WithTags("Games");
    }

    public static async Task<IResult> Handler(
        int gameId,
        StartGameParams actionParams,
        WssDbContext context,
        IAction<StartGameParams, Result<Game>> startGameAction
    )
    {
        actionParams = new StartGameParams(GameId: gameId);

        using var transaction = context.Database.BeginTransaction();

        var actionResult = await startGameAction.PerformAsync(actionParams);

        if (actionResult.IsFailed)
        {
            await transaction.RollbackAsync();
            return Results.BadRequest(new { Errors = actionResult.Errors.Select(e => e.Message) });
        }

        await transaction.CommitAsync();
        return Results.Ok();
    }
}
