using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;

public record GetChildrenDepartmentsByParentIdQuery(
    Guid ParentId,
    GetChildrenDepartmentByParentIdRequest Request)
    : IQuery;