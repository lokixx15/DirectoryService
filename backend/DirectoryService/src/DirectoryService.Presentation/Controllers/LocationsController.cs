using DirectoryService.Application.Departments.Features.RestoreDepartment;
using DirectoryService.Application.Locations.Features.CreateLocation;
using DirectoryService.Application.Locations.Features.GetLocations;
using DirectoryService.Application.Locations.Features.RestoreLocation;
using DirectoryService.Application.Locations.Features.SoftDeleteLocation;
using DirectoryService.Application.Locations.Features.UpdateLocation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Endpoints;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("directory/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<PaginationResponse<LocationDto>>> GetLocations(
        [FromServices] GetLocationsHandler handler,
        [FromQuery] GetLocationsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationsQuery(request);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet]
    [Route("summary")]
    public async Task<EndpointResult<PaginationResponse<LocationSummaryDto>>> GetLocations(
        [FromServices] GetLocationsSummaryHandler handler,
        [FromQuery] GetLocationsSummaryRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationsSummaryQuery(request);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> CreateLocation(
        [FromServices] CreateLocationHandler handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPatch("{locationId:guid}/restore")]
    public async Task<EndpointResult> RestoreDepartment(
        [FromRoute] Guid locationId,
        [FromServices] RestoreLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RestoreLocationCommand(locationId);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult> UpdateLocation(
        [FromRoute] Guid id,
        [FromServices] UpdateLocationHandler handler,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(id, request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> SoftDeleteLocation(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteLocationHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteLocationCommand(id);

        return await handler.Handle(command, cancellationToken);
    }
}
