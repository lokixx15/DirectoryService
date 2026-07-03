namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsRequest(
    string? Search,
    Guid? ParentId,
    Guid[]? LocationIds,
    Guid[]? ExcludeDepartmentIds,
    int Page = 1,
    int pageSize = 20,
    string OrderBy = "createdDate",
    string OrderDirection = "ASC");