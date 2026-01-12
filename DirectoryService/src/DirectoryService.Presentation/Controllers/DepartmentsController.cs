using DirectoryService.Application.Departments.Features;
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
}
