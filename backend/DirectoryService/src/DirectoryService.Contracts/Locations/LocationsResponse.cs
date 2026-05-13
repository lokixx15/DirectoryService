namespace DirectoryService.Contracts.Locations;

public record LocationsResponse(List<LocationDto> Locations, long? TotalCount = 0);