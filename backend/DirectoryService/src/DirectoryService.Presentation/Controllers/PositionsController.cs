using DirectoryService.Application.Departments.Features.RestoreDepartment;
using DirectoryService.Application.Departments.Features.RestorePosition;
using DirectoryService.Application.Positions.Features.CreatePosition;
using DirectoryService.Application.Positions.Features.GetPositions;
using DirectoryService.Application.Positions.Features.RestorePosition;
using DirectoryService.Application.Positions.Features.SoftDeletePosition;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Endpoints;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("directory/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<CursorPaginationResponse<PositionDto>>> GetPositions(
        [FromQuery] GetPositionsRequest request,
        [FromServices] GetPositionsHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPositionsQuery(request);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> CreatePosition(
        [FromServices] CreatePositionHandler handler,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPatch("{positionId:guid}/restore")]
    public async Task<EndpointResult> RestoreDepartment(
        [FromRoute] Guid positionId,
        [FromServices] RestorePositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RestorePositionCommand(positionId);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> SoftDeleteLocation(
        [FromRoute] Guid id,
        [FromServices] SoftDeletePositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeletePositionCommand(id);

        return await handler.Handle(command, cancellationToken);
    }
}
