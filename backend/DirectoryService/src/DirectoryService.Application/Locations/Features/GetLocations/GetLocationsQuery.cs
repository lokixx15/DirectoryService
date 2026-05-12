using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public record GetLocationsQuery(GetLocationsRequest Request)
    : IQuery;