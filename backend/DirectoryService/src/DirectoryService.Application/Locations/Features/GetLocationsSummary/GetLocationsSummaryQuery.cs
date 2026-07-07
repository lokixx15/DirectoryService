using DirectoryService.Contracts.Locations;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public record GetLocationsSummaryQuery(GetLocationsSummaryRequest Request)
    : IQuery;
