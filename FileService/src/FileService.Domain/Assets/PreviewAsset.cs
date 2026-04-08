using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.Assets;

public class PreviewAsset : MediaAsset
{
    // ef core
    private PreviewAsset() { }

    public const long MAX_SIZE = 10_485_760;
    public const string BUCKET = "previews";
    public const string RAW_PREFIX = "raw";
    public static readonly string[] AllowedExtensions = ["jpg", "jpeg", "png", "webp"];

    public PreviewAsset(
        Guid id,
        MediaData mediaData,
        AssetType assetType,
        MediaStatus mediaStatus,
        StorageKey rawKey,
        MediaOwner mediaOwner)
        : base(id, mediaData, assetType, mediaStatus, rawKey, mediaOwner) { }

    public static UnitResult<Error> ValidateForUpload(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
            return Error.Validation("file.extenstion.is.invalid", "File extension is not allowed.", nameof(mediaData.FileName));

        if (mediaData.ContentType.Category != MediaType.IMAGE)
            return Error.Validation("file.media.type.is.invalid", "File must be a video.", nameof(mediaData.ContentType));

        if (mediaData.Size > MAX_SIZE)
            return Error.Validation("file.length.is.invalid", $"File size exceeds {MAX_SIZE} limit.", nameof(mediaData.Size));

        return UnitResult.Success<Error>();
    }

    public override UnitResult<Error> CompleteProcessing(DateTime timestamp)
    {
        var markUploadedResult = MarkUploaded(timestamp);
        if (markUploadedResult.IsFailure)
            return markUploadedResult.Error;

        var markReadyResult = MarkReady(RawKey, timestamp);

        return UnitResult.Success<Error>();
    }
}