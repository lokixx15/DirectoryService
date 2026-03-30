using CSharpFunctionalExtensions;

namespace SharedService.SharedKernel;

public static class ResultsExtensions
{
    public static Result<TValue, Errors> ToErrorsResult<TValue>(
        Result<TValue, Error> result)
    {
        if (result.IsFailure)
            return Result.Failure<TValue, Errors>(result.Error);

        return Result.Success<TValue, Errors>(result.Value);
    }
}