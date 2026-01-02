using DirectoryService.Application.Locations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.EnpointResults;
using Microsoft.AspNetCore.Mvc;
namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> CreateLocation(
        [FromBody] CreateLocationDto request,
        CancellationToken cancellationToken)
    {
        return await _locationsService.Create(request, cancellationToken);
    }
}
