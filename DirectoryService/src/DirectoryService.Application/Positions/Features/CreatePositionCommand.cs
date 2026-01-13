using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Positions.Features;

public record CreatePositionCommand(CreatePositionDto CreatePositionDto) : ICommand;
