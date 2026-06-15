using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace DirectoryService.Application;

public sealed record Cursor
{
    public Guid Id { get; }

    public DateTime CreatedAt { get; }

    public Cursor(
        Guid id,
        DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public static string Encode(Guid id, DateTime createdAt)
    {
        var cursor = new Cursor(id, createdAt);
        string json = JsonSerializer.Serialize(cursor);
        return Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(json));
    }

    public static Cursor? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        try
        {
            string json = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(cursor));
            return JsonSerializer.Deserialize<Cursor>(json);
        }
        catch
        {
            return null;
        }
    }
}
