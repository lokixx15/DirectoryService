namespace DirectoryService.Presentation.Controllers;

public record GetRootDepartmentsWithChildrenRequest(
    PaginationRequest Pagination,
    int Prefetch = 3);
