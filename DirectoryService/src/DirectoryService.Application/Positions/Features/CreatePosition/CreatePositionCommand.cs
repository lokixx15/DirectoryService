using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Positions.Features.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand;