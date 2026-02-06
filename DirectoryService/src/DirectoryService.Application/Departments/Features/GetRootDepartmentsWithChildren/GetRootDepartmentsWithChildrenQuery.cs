using DirectoryService.Application.Abstractions;
using DirectoryService.Presentation.Controllers;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public record GetRootDepartmentsWithChildrenQuery(GetRootDepartmentsWithChildrenRequest Request) 
    :IQuery;