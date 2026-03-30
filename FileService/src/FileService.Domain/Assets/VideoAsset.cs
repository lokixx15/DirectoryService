using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.Assets;

public class VideoAsset : MediaAsset
{
    // ef core
    private VideoAsset() { }

    public const long MAX_SIZE = 5_368_709_120;
    public const string BUCKET = "videos";
    public const string RAW_PREFIX = "raw";
    public const string HLS_PREFIX = "hls";
    public const string MASTER_PLAYLIST_NAME = "master.m3u8";
    public static readonly string[] AllowedExtensions = ["mp4", "mkv", "avi", "mov"];

    public StorageKey HslRootKey { get; private set; } = null!;

    public VideoAsset(
        Guid id,
        MediaData mediaData,
        AssetType assetType,
        MediaStatus mediaStatus,
        StorageKey rawKey,
        MediaOwner mediaOwner,
        StorageKey hslRootKey)
        : base(id, mediaData, assetType, mediaStatus, rawKey, mediaOwner)
    {
        HslRootKey = hslRootKey;
    }

    public static UnitResult<Error> ValidateForUpload(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
            return Error.Validation("file.extenstion.is.invalid", "File extension is not allowed.", nameof(mediaData.FileName));

        if (mediaData.ContentType.Category != MediaType.VIDEO)
            return Error.Validation("file.media.type.is.invalid", "File must be a preview.", nameof(mediaData.ContentType));

        if (mediaData.Size > MAX_SIZE)
            return Error.Validation("file.length.is.invalid", $"File size exceeds {MAX_SIZE} limit.", nameof(mediaData.Size));

        return UnitResult.Success<Error>();
    }

    public override UnitResult<Error> CompleteProcessing(DateTime timestamp)
    {
        var appendSegmentResult = HslRootKey.AppendSegment(MASTER_PLAYLIST_NAME);
        if (appendSegmentResult.IsFailure)
            return appendSegmentResult.Error;

        var markReadyResult = MarkReady(appendSegmentResult.Value, timestamp);
        if (markReadyResult.IsFailure)
            return markReadyResult.Error;

        return UnitResult.Success<Error>();
    }
}