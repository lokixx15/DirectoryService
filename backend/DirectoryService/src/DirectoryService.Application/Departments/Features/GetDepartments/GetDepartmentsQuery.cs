using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public record GetDepartmentsSummaryQuery(GetDepartmentsSummaryRequest Request)
    : IQuery;