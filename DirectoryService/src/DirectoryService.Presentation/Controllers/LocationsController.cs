using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.Features;
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
        [FromBody] CreateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        return await handler.Handle(command, cancellationToken);
    }
}
