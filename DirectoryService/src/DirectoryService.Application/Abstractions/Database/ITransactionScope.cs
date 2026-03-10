using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Abstractions.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}