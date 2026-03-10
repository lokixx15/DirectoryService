using DirectoryService.Application.Positions.Features.CreatePosition;
using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Endpoints;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> CreatePosition(
        [FromServices] CreatePositionHandler handler,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);

        return await handler.Handle(command, cancellationToken);
    }
}