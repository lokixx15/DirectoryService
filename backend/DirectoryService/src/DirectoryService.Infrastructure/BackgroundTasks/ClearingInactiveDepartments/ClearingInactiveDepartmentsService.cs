using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.BackgroundTasks.ClearingInactiveDepartments;

public sealed class ClearingInactiveDepartmentsService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<ClearingInactiveDepartmentsOptions> _options;
    private readonly ILogger<ClearingInactiveDepartmentsService> _logger;

    public ClearingInactiveDepartmentsService(
        IServiceScopeFactory scopeFactory,
        IOptions<ClearingInactiveDepartmentsOptions> options,
        ILogger<ClearingInactiveDepartmentsService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Clearing inactive departments started");

        var cleaningInterval = _options.Value.CleaningInterval;

        while (!stoppingToken.IsCancellationRequested)
        {
            var cutOffDate = DateTime.UtcNow - _options.Value.MaxLifeCycleOfRemoteDepartment;

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
                await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                var deleteResult = await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                        WITH departments_to_delete AS (
                            SELECT *
                            FROM departments
                            WHERE deleted_at IS NOT NULL
                              AND deleted_at < {cutOffDate}
                              AND is_active = false
                            FOR UPDATE
                        ),
                        lock_descendants AS (
                            SELECT d.*
                            FROM departments AS d
                            JOIN departments_to_delete AS dtd ON d.path <@ dtd.path
                            FOR UPDATE
                        ),
                        delete_related_locations AS (
                            DELETE 
                            FROM department_location 
                            WHERE department_id = ANY (SELECT id FROM departments_to_delete)
                        ),
                        delete_related_positions AS (
                            DELETE
                            FROM department_position 
                            WHERE department_id = ANY (SELECT id FROM departments_to_delete)
                        ),
                        update_descendants_paths AS (
                            UPDATE departments AS d
                            SET path = subpath(dtd.path, 0, nlevel(dtd.path) - 1) || subpath(d.path, nlevel(dtd.path)),
                                depth = nlevel(subpath(dtd.path, 0, nlevel(dtd.path) - 1) || subpath(d.path, nlevel(dtd.path))) - 1,
                                parent_id = dtd.parent_id
                            FROM departments_to_delete AS dtd
                            WHERE d.path <@ dtd.path 
                              AND nlevel(d.path) != nlevel(dtd.path)
                        )
                        DELETE 
                        FROM departments 
                        WHERE id = ANY (SELECT id FROM departments_to_delete)
                        ",
                stoppingToken);

                await transaction.CommitAsync(stoppingToken);

                if (deleteResult > 0)
                    _logger.LogInformation("Inactive departments were cleaned");
                else
                    _logger.LogInformation("Departments to delete were not found");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Clearing inactive departments was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error during clearing inactive departments");
                throw;
            }

            await Task.Delay(cleaningInterval, stoppingToken);
        }
    }
}