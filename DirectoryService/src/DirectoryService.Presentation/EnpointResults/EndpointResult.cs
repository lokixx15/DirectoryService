using CSharpFunctionalExtensions;
using IResult = Microsoft.AspNetCore.Http.IResult;
using SharedKernel;

namespace DirectoryService.Presentation.EnpointResults;

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

    public static implicit operator EndpointResult<TValue>(Result<TValue, Errors> result) => new(result); 
}
