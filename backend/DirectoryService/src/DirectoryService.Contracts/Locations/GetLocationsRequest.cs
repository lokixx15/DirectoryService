namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    Guid[]? SelectedDepartmentIds,
    Guid[]? ExcludedDepartmentIds,
    string? Search,
    bool? IsActive,
    int Page = 1,
    int pageSize = 20,
    string? OrderBy = "createdDate",
    string OrderDirection = "ASC");