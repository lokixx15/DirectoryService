namespace DirectoryService.Contracts.Departments;

public record GetRootDepartmentsWithChildrenRequest(
    int Page = 1,
    int Size = 20,
    int Prefetch = 3);