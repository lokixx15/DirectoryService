using DirectoryService.Application.Departments.Features.CreateDepartment;
using DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;
using DirectoryService.Application.Departments.Features.UpdateDepartmentParent;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presentation.EnpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> CreateDepartment(
        [FromServices] CreateDepartmentHandler handler,
        [FromBody] CreateDepartmentDto createDepartmentDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(createDepartmentDto);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<EndpointResult> UpdateDepartmentLocations(
        [FromServices] UpdateDepartmentLocationsHadnler handler,
        [FromRoute] Guid departmentId,
        [FromBody] Guid[] locationIds,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, locationIds);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:guid}/parent")]
    public async Task<EndpointResult> UpdateDepartmentParent(
    [FromServices] UpdateDepartmentParentHandler handler,
    [FromRoute] Guid departmentId,
    [FromBody] Guid? parentId,
    CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentParentCommand(departmentId, parentId);

        return await handler.Handle(command, cancellationToken);
    }

}