using DirectoryService.Contracts.Positions;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Positions.Features.GetPositions;

public record GetPositionsQuery(GetPositionsRequest Request) : IQuery;