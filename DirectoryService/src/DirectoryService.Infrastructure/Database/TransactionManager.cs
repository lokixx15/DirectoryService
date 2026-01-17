using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;

namespace DirectoryService.Infrastructure.Database;

public class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(
        DirectoryServiceDbContext dbContext, 
        ILogger<TransactionManager> logger, 
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
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
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Changes were saved in the database");

            return UnitResult.Success<Error>();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                return GeneralErrors.ValueAlreadyExists("Location already exists");

            _logger.LogError(ex, "Database update error when saving changes");

            return Error.Failure("database", ex.Message);
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