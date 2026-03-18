using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.Assets;

public abstract class MediaAsset
{
    public Guid Id { get; protected set; }

    public MediaData MediaData { get; protected set; } = null!;

    public AssetType AssetType { get; protected set; }

    public MediaStatus MediaStatus { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime UpdatedAt { get; protected set; }

    public StorageKey RawKey { get; protected set; } = null!;

    public StorageKey FinalKey { get; protected set; } = StorageKey.None;

    public MediaOwner MediaOwner { get; protected set; } = null!;

    protected MediaAsset() { }

    protected MediaAsset(
        Guid id,
        MediaData mediaData,
        AssetType assetType,
        MediaStatus mediaStatus,
        StorageKey rawKey,
        MediaOwner mediaOwner)
    {
        Id = id;
        MediaData = mediaData;
        AssetType = assetType;
        MediaStatus = mediaStatus;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        RawKey = rawKey;
        MediaOwner = mediaOwner;
    }

    protected UnitResult<Error> MarkUploaded(DateTime uploadedAt)
    {
        if (MediaStatus == MediaStatus.UPLOADED)
            return UnitResult.Success<Error>();

        if (MediaStatus != MediaStatus.UPLOADING)
            return Error.Validation("file.state.transition", "Invalid state transition", nameof(MediaStatus));

        MediaStatus = MediaStatus.UPLOADED;
        UpdatedAt = uploadedAt;

        return UnitResult.Success<Error>();
    }

    protected UnitResult<Error> MarkReady(StorageKey finalKey, DateTime readyAt)
    {
        if (MediaStatus == MediaStatus.READY)
            return UnitResult.Success<Error>();

        if (MediaStatus != MediaStatus.UPLOADED)
            return Error.Validation("file.state.transition.is.invalid", "Invalid state transition", nameof(MediaStatus));

        MediaStatus = MediaStatus.READY;
        FinalKey = finalKey;
        UpdatedAt = readyAt;

        return UnitResult.Success<Error>();
    }

    protected UnitResult<Error> MarkFailed(DateTime failedAt)
    {
        if (MediaStatus != MediaStatus.UPLOADING && MediaStatus != MediaStatus.UPLOADED)
            return Error.Validation("file.state.transition.is.invalid", "Invalid state transition", nameof(MediaStatus));

        MediaStatus = MediaStatus.FAILED;
        UpdatedAt = failedAt;

        return UnitResult.Success<Error>();
    }

    protected UnitResult<Error> MarkDeleted(DateTime deletedAt)
    {
        if (MediaStatus == MediaStatus.DELETED)
            return UnitResult.Success<Error>();

        MediaStatus = MediaStatus.DELETED;
        UpdatedAt = deletedAt;

        return UnitResult.Success<Error>();
    }
}