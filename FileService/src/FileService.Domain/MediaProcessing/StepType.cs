namespace FileService.Domain.MediaProcessing;

public enum StepType
{
    INITIALIZE,
    EXTRACT_METADATA,
    GENERATE_HLS,
    UPLOAD_HLS,
    CLEANUP
}