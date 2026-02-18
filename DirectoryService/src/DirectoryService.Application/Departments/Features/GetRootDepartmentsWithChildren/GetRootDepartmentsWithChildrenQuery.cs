using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public record GetRootDepartmentsWithChildrenQuery(GetRootDepartmentsWithChildrenRequest Request) 
    :IQuery;