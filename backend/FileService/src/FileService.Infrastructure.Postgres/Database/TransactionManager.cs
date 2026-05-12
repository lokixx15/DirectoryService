using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.Postgres.Database;

public class TransactionManager : ITransactionManager
{
    private readonly FileServiceDbContext _dbContext;

    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(
        FileServiceDbContext dbContext,
        ILoggerFactory loggerFactory,
        ILogger<TransactionManager> logger)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;
        _logger = logger;
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            _logger.LogInformation("The transaction has been started");

            var transactionManagerLogger = _loggerFactory.CreateLogger<TransactionScope>();

            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), transactionManagerLogger);

            _logger.LogInformation("The transactionScope was created");

            return transactionScope;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to begin transaction");
            return Error.Failure("database", "Failed to begin transaction");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Changes were saved in the database");

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when saving changes");
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes");
            return Error.Failure("database", "Failed to save changes");
        }
    }
}