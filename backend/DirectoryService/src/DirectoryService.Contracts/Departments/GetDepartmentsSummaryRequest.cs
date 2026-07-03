namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsSummaryRequest(
    string? Search,
    int Page = 1,
    int pageSize = 20);