namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsSummaryRequest(
    string? Search,
    Guid? ExcludedId,
    int Page = 1,
    int PageSize = 20);