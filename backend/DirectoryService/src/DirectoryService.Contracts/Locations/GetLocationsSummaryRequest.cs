namespace DirectoryService.Contracts.Locations;

public record GetLocationsSummaryRequest(
    string? Search,
    int Page = 1,
    int pageSize = 20);
