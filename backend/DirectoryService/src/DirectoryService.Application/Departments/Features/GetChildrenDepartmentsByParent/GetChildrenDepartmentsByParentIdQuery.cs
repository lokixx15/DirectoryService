using DirectoryService.Contracts;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;

public record GetChildrenDepartmentsByParentIdQuery(
    Guid ParentId,
    int Page,
    int Size)
    : IQuery;