using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Positions.Features.SoftDeletePosition;

public record SoftDeletePositionCommand(Guid Id) : ICommand;
