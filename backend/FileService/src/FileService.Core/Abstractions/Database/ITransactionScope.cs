using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Core.Abstractions.Database;

public interface ITransactionScope
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}