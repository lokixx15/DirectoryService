using CSharpFunctionalExtensions;
using FileService.Domain;
using FileService.Domain.MediaProcessing;
using SharedService.SharedKernel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileService.VideoProcessing.FfmpegProcess;

public static class FfprobeOutputParser
{
    public static Result<Metadata, Error> Parse(string jsonOutput)
    {
        if (string.IsNullOrWhiteSpace(jsonOutput))
            return FileErrors.InvalidFfprobeOutput("Ffprobe output is empty");

        FfprobeResponse? response;

        try
        {
            response = JsonSerializer.Deserialize<FfprobeResponse>(jsonOutput);
        }
        catch (JsonException ex)
        {
            return FileErrors.InvalidFfprobeOutput($"Failed to parse ffprobe output: {ex.Message}");
        }

        if (response == null)
            return FileErrors.InvalidFfprobeOutput($"Failed to parse ffprobe output: deserialization resulted in null");

        StreamInfo? streamInfo = response.Streams?.FirstOrDefault();

        if (streamInfo == null)
            return FileErrors.InvalidFfprobeOutput("No stream information found in ffprobe output");

        if (streamInfo.Width == null)
            return FileErrors.InvalidFfprobeOutput("Stream width is missing in ffprobe output");

        if (streamInfo.Height == null)
            return FileErrors.InvalidFfprobeOutput("Stream height is missing in ffprobe output");

        if (streamInfo.CodecName == null)
            return FileErrors.InvalidFfprobeOutput("Stream codec name is missing in ffprobe output");

        FormatInfo? formatInfo = response.Format;

        if (formatInfo == null)
            return FileErrors.InvalidFfprobeOutput("Format information is missing in ffprobe output");

        if (formatInfo.Duration == null || formatInfo.Duration <= 0)
            return FileErrors.InvalidFfprobeOutput("Invalid duration in ffprobe output");

        var duration = TimeSpan.FromSeconds(formatInfo.Duration.Value);

        return Metadata.Create(
            duration,
            streamInfo.Width.Value,
            streamInfo.Height.Value,
            streamInfo.CodecName);
    }

    private sealed class FfprobeResponse
    {
        [JsonPropertyName("streams")]
        public List<StreamInfo>? Streams { get; set; }

        [JsonPropertyName("format")]
        public FormatInfo? Format { get; set; }
    }

    private sealed class StreamInfo
    {
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("codec_name")]
        public string? CodecName { get; set; }
    }

    private sealed class FormatInfo
    {
        [JsonPropertyName("duration")]
        [JsonConverter(typeof(StringToDoubleConverter))]
        public double? Duration { get; set; }
    }

    private sealed class StringToDoubleConverter : JsonConverter<double?>
    {
        public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? str = reader.GetString();
                if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                    return result;

                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDouble();

            return null;
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}