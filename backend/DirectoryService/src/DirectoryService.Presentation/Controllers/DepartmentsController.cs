using DirectoryService.Application.Departments.Features.AttachDepartmentVideo;
using DirectoryService.Application.Departments.Features.CreateDepartment;
using DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;
using DirectoryService.Application.Departments.Features.GetDepartments;
using DirectoryService.Application.Departments.Features.GetDepartmentsWithMostPositions;
using DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;
using DirectoryService.Application.Departments.Features.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;
using DirectoryService.Application.Departments.Features.UpdateDepartmentParent;
using DirectoryService.Application.Locations.Features.GetLocations;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Endpoints;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("directory/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpGet]
    [Route("summary")]
    public async Task<EndpointResult<PaginationResponse<DepartmentSummaryDto>>> GetDepartmentsSummary(
        [FromQuery] GetDepartmentsSummaryRequest request,
        [FromServices] GetDepartmentsSummaryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDepartmentsSummaryQuery(request);
        return await handler.Handle(query, cancellationToken);
    }

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
        [FromServices] GetChildrenDepartmentsByParentIdHandler handler,
        CancellationToken cancellationToken,
        [FromQuery] int Page = 1,
        [FromQuery] int Size = 20)
    {
        var query = new GetChildrenDepartmentsByParentIdQuery(parentId, Page, Size);

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

    [HttpPatch("{departmentId:guid}/video")]
    public async Task<EndpointResult> UpdateDepartmentParent(
        [FromRoute] Guid departmentId,
        [FromBody] AttachDepartmentVideoRequest request,
        [FromServices] AttachDepartmentVideoHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new AttachDepartmentVideoCommand(departmentId, request);

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