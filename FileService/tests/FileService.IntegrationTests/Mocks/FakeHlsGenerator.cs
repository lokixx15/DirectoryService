using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.FfmpegProcess;
using SharedService.SharedKernel;

namespace FileService.IntegrationTests.Mocks;

public class FakeHlsGenerator : IFfmpegProcessRunner
{
    private const int SEGMENTS_PER_QUALITY = 3;
    private static readonly string[] _qualities = ["360p", "720p", "1080p"];

    private readonly Metadata _fakeMetadata = Metadata.Create(
        duration: TimeSpan.FromSeconds(25),
        width: 1920,
        height: 1080,
        codec: "h264").Value;

    public Task<Result<Metadata, Error>> ExtractMetadataAsync(
        string inputFileUrl,
        CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success<Metadata, Error>(_fakeMetadata));

    public Task<UnitResult<Error>> GenerateHlsAsync(
        string inputFileUrl,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDirectory);
        CreateMasterPlaylist(outputDirectory);

        foreach(string quality in _qualities)
        {
            CreateVariantPlaylist(outputDirectory, quality);
            CreateSegments(outputDirectory, quality);
        }

        return Task.FromResult(UnitResult.Success<Error>());
    }

    private static void CreateMasterPlaylist(string outputDirectory)
    {
        string content = """
            #EXTM3U
            #EXT-X-VERSION:3

            #EXT-X-STREAM-INF:BANWIDTH=2000000,RESOLUTION=640x360,NAME="360p"
            360p_stream.m3u8

            #EXT-X-STREAM-INF:BANWIDTH=3000000,RESOLUTION=1280x720,NAME="720p"
            720p_stream.m3u8

            #EXT-X-STREAM-INF:BANWIDTH=5000000,RESOLUTION=1920x1080,NAME="1080p"
            1080p_stream.m3u8
            """;

        string masterPath = Path.Combine(outputDirectory, VideoAsset.MASTER_PLAYLIST_NAME);
        File.WriteAllText(masterPath, content);
    }

    private static void CreateVariantPlaylist(string outputDirectory, string quality)
    {
        string segmentList = string.Join(
            Environment.NewLine,
            Enumerable.Range(1, SEGMENTS_PER_QUALITY)
                .Select(i => $"#EXTINF:4.000,{Environment.NewLine}{quality}_{i:06}.ts"));

        string content = $"""
            #EXTM3U
            #EXT-X-VERSION:3
            #EXT-X-TARGETDURATION:4
            #EXT-X-MEDIA-SEQUENCE:0
            #EXT-X-PLAYLIST-TYPE:VOD

            {segmentList}

            #EXT-X-ENDLIST
            """;

        string playListPath = Path.Combine(outputDirectory, $"{quality}_stream.m3u8");
        File.WriteAllText(playListPath, content);
    }

    private static void CreateSegments(string outputDirectory, string quality)
    {
        byte[] fakeSegmentData = CreateFakeTsSegment(quality);

        for (int i = 1; i <= SEGMENTS_PER_QUALITY; i++)
        {
            string segmentName = $"{quality}_{i:06}.ts";
            string segmentPath = Path.Combine(outputDirectory, segmentName);
            File.WriteAllBytes(segmentPath, fakeSegmentData);
        }
    }

    private static byte[] CreateFakeTsSegment(string quality)
    {
        byte[] data = new byte[188 * 10];
        for (int i = 0; i < 10; i++)
        {
            data[i * 188] = 0x47;
        }

        byte[] qualityBytes = System.Text.Encoding.UTF8.GetBytes(quality);
        Array.Copy(qualityBytes, 0, data, 4, Math.Min(qualityBytes.Length, 10));

        return data;
    }
}