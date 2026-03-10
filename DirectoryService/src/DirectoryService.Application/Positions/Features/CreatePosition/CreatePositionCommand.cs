using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Positions.Features.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand;