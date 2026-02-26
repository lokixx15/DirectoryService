using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Data;

namespace DirectoryService.Infrastructure.Database;

public sealed class TransactionScope : ITransactionScope
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