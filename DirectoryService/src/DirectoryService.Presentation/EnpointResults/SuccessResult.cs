using System.Net;

namespace DirectoryService.Presentation.EnpointResults;

public sealed class SuccessResult : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok();

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        await httpContext.Response.WriteAsJsonAsync(envelope);
    }
}

public class SuccessResult<TValue> : IResult
{
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

        await httpContext.Response.WriteAsJsonAsync(envelope);
    }
}