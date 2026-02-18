using DirectoryService.Application.Departments.Features.CreateDepartment;
using DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;
using DirectoryService.Application.Departments.Features.GetDepartmentsWithMostPositions;
using DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;
using DirectoryService.Application.Departments.Features.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;
using DirectoryService.Application.Departments.Features.UpdateDepartmentParent;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presentation.EnpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    [Route("top-positions")]
    public async Task<EndpointResult<IReadOnlyList<DepartmentDto>>> GetDepartmentsWithMostPositions(
        [FromServices] GetDepartmentsWithMostPositionsHandler handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(cancellationToken);

    [HttpGet]
    [Route("roots")]
    public async Task<EndpointResult<IReadOnlyList<DepartmentDto>>> GetRootDepartmentsWithChildren(
    [FromQuery] GetRootDepartmentsWithChildrenRequest request,
    [FromServices] GetRootDepartmentsWithChildrenHandler handler,
    CancellationToken cancellationToken)
    {
        var query = new GetRootDepartmentsWithChildrenQuery(request);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet]
    [Route("{parentId:guid}/children")]
    public async Task<EndpointResult<PaginationResponse<DepartmentDto>>> GetChildrenDepartmentsByRootId(
    [FromRoute] Guid parentId,
    [FromQuery] PaginationRequest request,
    [FromServices] GetChildrenDepartmentsByParentIdHandler handler,
    CancellationToken cancellationToken)
    {
        var query = new GetChildrenDepartmentsByParentIdQuery(parentId, request);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> CreateDepartment(
        [FromServices] CreateDepartmentHandler handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<EndpointResult> UpdateDepartmentLocations(
        [FromServices] UpdateDepartmentLocationsHadnler handler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId:guid}/parent")]
    public async Task<EndpointResult> UpdateDepartmentParent(
    [FromServices] UpdateDepartmentParentHandler handler,
    [FromRoute] Guid departmentId,
    [FromBody] UpdateDepartmentParentRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentParentCommand(departmentId, request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpDelete]
    [Route("{departmentId:guid}")]
    public async Task<EndpointResult> SoftDeleteDepartment(
        [FromRoute] Guid departmentId,
        [FromServices] SoftDeleteDepartmentHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteDepartmentCommand(departmentId);

        return await handler.Handle(command, cancellationToken);
    }
}