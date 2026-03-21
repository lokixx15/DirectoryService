using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record StorageKey
{
    // ef core
    private StorageKey() { }

    public string Bucket { get; } = string.Empty;

    public string Prefix { get; } = string.Empty;

    public string Key { get; } = string.Empty;

    public string Value { get; } = string.Empty;

    public string FullPath { get; } = string.Empty;

    public static StorageKey None => new("raw", null!, string.Empty);

    private StorageKey(
        string bucket,
        string prefix,
        string key)
    {
        Bucket = bucket;
        Prefix = prefix;
        Key = key;
        Value = string.IsNullOrWhiteSpace(prefix) ? $"{key}" : $"{prefix}/{key}";
        FullPath = $"{bucket}/{Value}";
    }

    public static Result<StorageKey, Error> Create(
        string bucket,
        string? prefix,
        string key)
    {
        var bucketNormalizeResult = NormalizeSegment(bucket);
        if (bucketNormalizeResult.IsFailure)
            return bucketNormalizeResult.Error;

        var keyNormalizeResult = NormalizeSegment(key);
        if (keyNormalizeResult.IsFailure)
            return keyNormalizeResult.Error;

        if (keyNormalizeResult.Value.Contains('/'))
            return GeneralErrors.ValueIsNotValid("Key cannot contain /", nameof(key));

        var prefixNormalizeResult = NormalizePrefix(prefix);
        if (prefixNormalizeResult.IsFailure)
            return prefixNormalizeResult.Error;

        return new StorageKey(bucketNormalizeResult.Value, prefixNormalizeResult.Value, keyNormalizeResult.Value);
    }

    public Result<StorageKey, Error> AppendSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return GeneralErrors.ValueIsNullOrWhitespace(segment);

        var newPrefix = Value;
        return Create(Bucket, newPrefix, segment);
    }

    private static Result<string, Error> NormalizePrefix(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return string.Empty;

        string[] parts = prefix.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<string> normalizedParts = [];

        foreach (string part in parts)
        {
            var normalizedPartResult = NormalizeSegment(part);
            if (normalizedPartResult.IsFailure)
                return normalizedPartResult.Error;

            if (normalizedPartResult.Value.Contains('/'))
                return GeneralErrors.ValueIsNotValid("Prefix part cannot contain /");

            if (!string.IsNullOrWhiteSpace(normalizedPartResult.Value))
                normalizedParts.Add(normalizedPartResult.Value);
        }

        return string.Join('/', normalizedParts);
    }

    private static Result<string, Error> NormalizeSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return GeneralErrors.ValueIsNullOrWhitespace(segment);

        segment = segment.Replace('\\', '/').Trim([' ', '/']);
        segment = Regex.Replace(segment, @"\s+", string.Empty);

        return segment;
    }
}