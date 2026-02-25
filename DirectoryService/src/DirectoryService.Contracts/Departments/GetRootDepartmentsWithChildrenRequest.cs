namespace DirectoryService.Contracts.Departments;

public record GetRootDepartmentsWithChildrenRequest(
    PaginationRequest Pagination,
    int Prefetch = 3);