namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    Guid[]? DepartmentIds,
    string? Search,
    bool? IsActive,
    PaginationRequest Pagination,
    string? OrderBy = "createdDate",
    string OrderDirection = "ASC");
