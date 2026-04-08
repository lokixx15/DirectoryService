using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using SharedService.SharedKernel;

namespace FileService.Communication;

public interface IFileService
{
    Task<Result<GetMediaAssetInfoResponse, Error>> GetMediaAssetInfo(Guid mediaAssetId, CancellationToken cancellationToken);

    Task<Result<GetMediaAssetsInfoResponse, Error>> GetMediaAssetsInfo(GetMediaAssetsInfoRequest request, CancellationToken cancellationToken);
}