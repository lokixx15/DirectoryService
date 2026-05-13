using CSharpFunctionalExtensions;
using FileService.Domain;
using FileService.Domain.Assets;
using SharedService.SharedKernel;

namespace FileService.Core;

public class MediaAssetFactory : IMediaAssetFactory
{
    public Result<MediaAsset, Error> CreateForUpload(
        MediaData mediaData,
        AssetType assetType,
        MediaOwner mediaOwner)
    {
        switch (assetType)
        {
            case AssetType.VIDEO:
                var videoResult = CreateVideoForUpload(mediaData, mediaOwner);
                return videoResult.IsFailure ? videoResult.Error : videoResult.Value;
            case AssetType.PREVIEW:
                var previewResult = CreatePreviewForUpload(mediaData, mediaOwner);
                return previewResult.IsFailure ? previewResult.Error : previewResult.Value;
            default:
                throw new ArgumentException("Asset type is not valid");
        }
    }

    public Result<VideoAsset, Error> CreateVideoForUpload(MediaData mediaData, MediaOwner owner)
    {
        var validationResult = VideoAsset.ValidateForUpload(mediaData);

        if (validationResult.IsFailure)
            return validationResult.Error;

        var id = Guid.NewGuid();
        var rawKey = StorageKey.Create(VideoAsset.BUCKET, VideoAsset.RAW_PREFIX, id.ToString()).Value;
        var hslRootKey = StorageKey.Create(VideoAsset.BUCKET, VideoAsset.HLS_PREFIX, id.ToString()).Value;

        return new VideoAsset(id, mediaData, AssetType.VIDEO, MediaStatus.UPLOADING, rawKey, owner, hslRootKey);
    }

    public Result<PreviewAsset, Error> CreatePreviewForUpload(MediaData mediaData, MediaOwner owner)
    {
        var validationResult = PreviewAsset.ValidateForUpload(mediaData);

        if (validationResult.IsFailure)
            return validationResult.Error;

        var id = Guid.NewGuid();
        var rawKey = StorageKey.Create(PreviewAsset.BUCKET, PreviewAsset.RAW_PREFIX, id.ToString()).Value;

        return new PreviewAsset(id, mediaData, AssetType.PREVIEW, MediaStatus.UPLOADING, rawKey, owner);
    }
}