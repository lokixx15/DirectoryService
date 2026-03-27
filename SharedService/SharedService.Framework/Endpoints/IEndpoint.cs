using Microsoft.AspNetCore.Routing;

namespace SharedService.Framework.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}