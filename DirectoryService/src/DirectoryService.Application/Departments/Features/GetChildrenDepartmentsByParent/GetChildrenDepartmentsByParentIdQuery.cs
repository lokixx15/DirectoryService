using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;

public record GetChildrenDepartmentsByParentIdQuery(Guid ParentId, PaginationRequest Request)
    : IQuery;