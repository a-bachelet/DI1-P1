namespace Server.Endpoints.Contracts;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

