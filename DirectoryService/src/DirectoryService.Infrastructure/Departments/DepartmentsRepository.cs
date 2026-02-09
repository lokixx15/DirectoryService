using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;
using System.Linq;

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

    public async Task<Result<Department, Error>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive, cancellationToken);

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

    public async Task<Result<Department, Error>> GetByIdWithLockAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _dbContext.Departments.FromSqlInterpolated($"""
                SELECT *
                FROM departments 
                WHERE id = {id} AND is_active = true
                FOR UPDATE
                """)
                .FirstOrDefaultAsync(cancellationToken);

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

    public async Task<UnitResult<Error>> LockDescendants(
        DepartmentPath oldDepartmentPath, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                SELECT path, depth
                FROM departments 
                WHERE path <@ {0}::ltree
                FOR UPDATE", 
                [oldDepartmentPath.Value], 
                cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lock descendants of {Path}", oldDepartmentPath);
            return GeneralErrors.DataLockFailed(ex.Message);
        }
    }

    public async Task<UnitResult<Error>> ExistsAsync(
        IEnumerable<Guid> ids, 
        CancellationToken cancellationToken = default)
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

    public async Task<Result<Guid, Error>> AddAsync(
        Department department, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(department, cancellationToken);
        _logger.LogInformation("Department was addedd to the database");

        return department.Id;
    }

    public async Task<UnitResult<Error>> AddLocationsToDepartmentAsync(
        List<DepartmentLocation> locations, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.DepartmentLocations.AddRangeAsync(locations, cancellationToken);
        _logger.LogInformation("Locations were addedd to the database");

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateDepartmentDescendantsParentAsync(
        DepartmentPath? newUpdatedDepartmentPath,
        DepartmentPath oldUpdatedDepartmentPath,
        CancellationToken cancellationToken = default)
    {
        var newPath = newUpdatedDepartmentPath?.Value ?? "";

        var sql = """
                        UPDATE departments
                        SET path = @updatedDepartmentPath::ltree
                        || subpath(path, nlevel(@oldUpdatedDepartmentPath::ltree)),
                            depth = nlevel(@updatedDepartmentPath::ltree
                        || subpath(path, nlevel(@oldUpdatedDepartmentPath::ltree))) - 1
                        WHERE path <@ @oldUpdatedDepartmentPath::ltree
                        AND nlevel(path) != nlevel(@oldUpdatedDepartmentPath::ltree);
                        """;

        await _dbContext.Database.ExecuteSqlRawAsync(sql,
            [new NpgsqlParameter("@updatedDepartmentPath", newPath),
            new NpgsqlParameter("@oldUpdatedDepartmentPath", oldUpdatedDepartmentPath.Value)], 
            cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateDescendantsPathsAsync(
    DepartmentPath newParentPath,
    DepartmentPath oldParentPath,
    CancellationToken cancellationToken = default)
    {
        var sql = """
                        UPDATE departments
                        SET path = @newParentPath::ltree || subpath(path, nlevel(@newParentPath::ltree))
                        WHERE path <@ @oldParentPath::ltree
                        AND nlevel(path) != nlevel(@oldParentPath::ltree);
                        """;

        await _dbContext.Database.ExecuteSqlRawAsync(sql,
            [new NpgsqlParameter("@newParentPath", newParentPath.Value),
            new NpgsqlParameter("@oldParentPath", oldParentPath.Value)],
            cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteLocationsByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.DepartmentLocations
            .Where(d => d.DepartmentId == departmentId)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("Locations were deleted from the database");

        return UnitResult.Success<Error>();
    }
}