namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    Guid[]? DepartmentIds,
    string? Search,
    bool? IsActive,
    int Page = 1,
    int pageSize = 20,
    string? OrderBy = "createdDate",
    string OrderDirection = "ASC");