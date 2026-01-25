using DirectoryService.Application.Locations.Features.CreateLocation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.EnpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> CreateLocation(
        [FromServices] CreateLocationHandler handler,
        [FromBody] CreateLocationDto createLocationDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(createLocationDto);

        return await handler.Handle(command, cancellationToken);
    }
}