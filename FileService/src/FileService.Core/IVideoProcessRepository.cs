using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using FileService.Domain.MediaProcessing;
using SharedService.SharedKernel;

namespace FileService.Core;

public interface IVideoProcessRepository
{
    Task<Result<VideoProcess, Error>> GetByAsync(Expression<Func<VideoProcess, bool>> predicate, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> AddAsync(VideoProcess videoProcess, CancellationToken cancellationToken);

}