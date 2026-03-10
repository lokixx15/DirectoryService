using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public record GetRootDepartmentsWithChildrenQuery(GetRootDepartmentsWithChildrenRequest Request)
    :IQuery;