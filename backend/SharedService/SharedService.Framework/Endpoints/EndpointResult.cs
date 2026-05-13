using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using SharedService.SharedKernel;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace SharedService.Framework.Endpoints;

public sealed class EndpointResult : IResult
{
    private readonly IResult _result;

    public EndpointResult(UnitResult<Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult()
            : new ErrorsResult(result.Error);
    }

    public EndpointResult(UnitResult<Errors> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult()
            : new ErrorsResult(result.Error);
    }

    public async Task ExecuteAsync(HttpContext httpContext) => await _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult(UnitResult<Error> result) => new(result);

    public static implicit operator EndpointResult(UnitResult<Errors> result) => new(result);

    public EndpointResult ToEndpointResult(UnitResult<Error> result) => new(result);

    public EndpointResult ToEndpointResult(UnitResult<Errors> result) => new(result);
}

public sealed class EndpointResult<TValue> : IResult
{
    private readonly IResult _result;

    public EndpointResult(Result<TValue, Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult(result.Error);
    }

    public EndpointResult(Result<TValue, Errors> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult(result.Error);
    }

    public async Task ExecuteAsync(HttpContext httpContext) =>
        await _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result) => new(result);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Errors> result) => new(result);

    public EndpointResult ToEndpointResult(UnitResult<Error> result) => new(result);

    public EndpointResult ToEndpointResult(UnitResult<Errors> result) => new(result);
}