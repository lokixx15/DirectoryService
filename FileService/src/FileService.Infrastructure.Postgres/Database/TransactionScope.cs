using System.Data;
using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.Database;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.Postgres.Database;

public class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _dbTransaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(
        IDbTransaction dbTransaction,
        ILogger<TransactionScope> logger)
    {
        _dbTransaction = dbTransaction;
        _logger = logger;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _dbTransaction.Commit();
            _logger.LogInformation("The transaction was committed");

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to commit transaction");
            return Error.Failure("database", "Failed to commit transaction");
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _dbTransaction.Rollback();
            _logger.LogInformation("The transaction was rollbacked");

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to rollback transaction");
            return Error.Failure("database", "Failed to rollback transaction");
        }
    }

    public void Dispose()
    {
        _dbTransaction.Dispose();
    }
}