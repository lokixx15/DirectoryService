using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using SharedService.SharedKernel;

namespace FileService.Domain;

public interface IMediaAssetFactory
{
    Result<MediaAsset, Error> CreateForUpload(MediaData mediaData, AssetType assetType, MediaOwner mediaOwner);

    Result<VideoAsset, Error> CreateVideoForUpload(MediaData mediaData, MediaOwner owner);

    Result<PreviewAsset, Error> CreatePreviewForUpload(MediaData mediaData, MediaOwner owner);
}