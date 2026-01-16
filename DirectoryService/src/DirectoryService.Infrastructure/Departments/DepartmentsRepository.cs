using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Infrastructure.Departments;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(
        DirectoryServiceDbContext dbContext, 
        ILogger<DepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                _logger.LogError("Department with id {Id} was not found in the database", id);
                return GeneralErrors.EntityNotFound("Department");
            }

            _logger.LogInformation("Department with id {Id} was obtained from the database", id);
            return department;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while reading department with id {Id}", id);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when reading department with id {Id}", id);
            return GeneralErrors.DatabaseReadFailed(ex.Message);
        }
    }

    public async Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var idList = ids.ToList();

            var existingsIds = await _dbContext.Departments
                .AsNoTracking()
                .Where(d => idList.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var nonexistentIds = idList.Except(existingsIds).ToList();

            return nonexistentIds.Any()
                ? GeneralErrors.EntityNotFound("DepartmentIds", $"Departments with ids {string.Join(",", nonexistentIds)}")
                : UnitResult.Success<Error>();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled checking the existence of departments with ids {Ids}", ids);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when checking the existence of departments with ids {Ids}", ids);
            return GeneralErrors.DatabaseReadFailed("Departments existence check failed");
        }
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(department, cancellationToken);
        _logger.LogInformation("Department was addedd to the database");

        return department.Id;
    }

    public async Task<UnitResult<Error>> AddLocationsToDepartmentAsync(List<DepartmentLocation> locations, CancellationToken cancellationToken)
    {
        await _dbContext.DepartmentLocations.AddRangeAsync(locations, cancellationToken);
        _logger.LogInformation("Locations were addedd to the database");

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteLocationsByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        await _dbContext.DepartmentLocations
            .Where(d => d.DepartmentId == departmentId)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("Locations were deleted from the database");

        return UnitResult.Success<Error>();
    }
}