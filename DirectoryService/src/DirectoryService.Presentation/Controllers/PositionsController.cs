using DirectoryService.Application.Positions.Features.CreatePosition;
using DirectoryService.Contracts.Positions;
using DirectoryService.Presentation.EnpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> CreatePosition(
        [FromServices] CreatePositionHandler handler,
        [FromBody] CreatePositionDto createPositionDto,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(createPositionDto);

        return await handler.Handle(command, cancellationToken);
    }
}