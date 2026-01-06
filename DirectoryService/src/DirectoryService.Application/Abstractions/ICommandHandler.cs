using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Application.Abstractions;

public interface ICommandHandler<T, in TCommand>
    where TCommand : ICommand
{
    Task<Result<T, Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}
