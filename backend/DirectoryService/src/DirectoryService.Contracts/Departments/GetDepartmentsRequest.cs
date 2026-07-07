namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsRequest(
    string? Search,
    Guid[]? LocationIds,
    Guid[]? DepartmentIds,
    Guid[]? ExcludeDepartmentIds,
    bool? IsActive,
    int Page = 1,
    int pageSize = 20,
    string OrderBy = "createdDate",
    string OrderDirection = "ASC");
