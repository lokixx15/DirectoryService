namespace DirectoryService.Contracts.Departments;

public record GetRootDepartmentsWithChildrenRequest(
    Guid[]? DepartmentIds,
    Guid[]? ExcludedDepartmentIds,
    bool IsActiveOnly,
    int Page = 1,
    int Size = 20,
    int Prefetch = 3);
