using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using SharedService.SharedKernel;

namespace FileService.Core;

public interface IMediaRepository
{
    Task<Result<MediaAsset, Error>> GetByAsync(Expression<Func<MediaAsset, bool>> predicate, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteById(Guid mediaAssetId, CancellationToken cancellationToken);
}