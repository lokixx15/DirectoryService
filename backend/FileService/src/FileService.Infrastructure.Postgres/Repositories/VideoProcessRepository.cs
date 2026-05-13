using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Domain.MediaProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.Postgres.Repositories;

public class VideoProcessRepository : IVideoProcessRepository
{
    private readonly FileServiceDbContext _dbContext;
    private readonly ILogger<VideoProcessRepository> _logger;

    public VideoProcessRepository(
        FileServiceDbContext dbContext,
        ILogger<VideoProcessRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<VideoProcess, Error>> GetByAsync(
        Expression<Func<VideoProcess, bool>> predicate,
        CancellationToken cancellationToken)
    {
        try
        {
            var videoProcess = await _dbContext.VideoProcesses
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (videoProcess == null)
            {
                _logger.LogError("Video process was not found in the database");
                return GeneralErrors.EntityNotFound("Media asset");
            }

            _logger.LogInformation("Video process was obtained from the database");
            return videoProcess;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while reading video process");
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when reading video process");
            return GeneralErrors.DatabaseReadFailed(ex.Message);
        }
    }

    public async Task<Result<Guid, Error>> AddAsync(
        VideoProcess videoProcess,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.VideoProcesses.AddAsync(videoProcess, cancellationToken);
            _logger.LogInformation("Video process was added to the database");

            return videoProcess.Id;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when adding video process with id {VideoProcessId}", videoProcess.Id);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add video process with id {VideoProcessId}", videoProcess.Id);
            return GeneralErrors.DatabaseAddFailed("Failed to add video process");
        }
    }
}