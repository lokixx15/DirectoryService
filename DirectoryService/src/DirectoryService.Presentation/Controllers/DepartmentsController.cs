using DirectoryService.Application.Departments.Features.CreateDepartment;
using DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;
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
        [FromBody] CreateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<EndpointResult> UpdateDepartmentLocations(
        [FromServices] UpdateDepartmentLocationsHadnler handler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, request);

        return await handler.Handle(command, cancellationToken);
    }
}