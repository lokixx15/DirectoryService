using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Core.Features;
using SharedService.SharedKernel;

namespace FileService.Communication;

public interface IFileService
{
    Task<Result<GetMediaAssetInfoResponse, Error>> GetMediaAssetInfo(Guid mediaAssetId, CancellationToken cancellationToken);

    Task<Result<GetMediaAssetsInfoResponse, Error>> GetMediaAssetsInfo(GetMediaAssetsInfoRequest request, CancellationToken cancellationToken);

    Task<Result<CheckVideoExistenceResponse, Error>> CheckVideoExistence(Guid videoId, CancellationToken cancellationToken);
}