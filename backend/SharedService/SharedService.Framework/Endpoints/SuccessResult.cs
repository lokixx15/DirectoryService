using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SharedService.SharedKernel;

namespace SharedService.Framework.Endpoints;

public sealed class SuccessResult : IResult
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok();

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        await httpContext.Response.WriteAsJsonAsync(envelope, JsonOptions);
    }
}

public class SuccessResult<TValue> : IResult
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly TValue _value;

    public SuccessResult(TValue value)
    {
        _value = value;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope<TValue>.Ok(_value);

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        await httpContext.Response.WriteAsJsonAsync(envelope, JsonOptions);
    }
}