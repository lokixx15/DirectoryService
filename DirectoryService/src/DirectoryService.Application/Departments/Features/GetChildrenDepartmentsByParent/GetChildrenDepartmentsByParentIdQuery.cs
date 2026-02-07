using DirectoryService.Application.Abstractions;
using DirectoryService.Presentation.Controllers;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByRootId;

public record GetChildrenDepartmentsByParentIdQuery(Guid ParentId, PaginationRequest Request)
    : IQuery;