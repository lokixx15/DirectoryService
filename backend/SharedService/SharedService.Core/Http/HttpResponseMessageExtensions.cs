using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.IntegrationTests.Features;

public static class HttpResponseMessageExtensions
{
    public static async Task<Result<TResponse, Error>> HandleResponseAsync<TResponse>(
        this HttpResponseMessage httpReponseMessage,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        try
        {
            if (!httpReponseMessage.IsSuccessStatusCode)
            {
                return Result.Failure<TResponse, Error>(Error.Failure("http.status.code",
                    $"Request failed with status code: {httpReponseMessage.StatusCode}"));
            }

            var response = await httpReponseMessage.Content.ReadFromJsonAsync<Envelope<TResponse>>(cancellationToken);

            if (response is null)
            {
                return Result.Failure<TResponse, Error>(Error.Failure("deserialization.null",
                    "Response deserialization returned null"));
            }

            if (response.Result is null)
            {
                return Result.Failure<TResponse, Error>(Error.Failure("envelope.result.null",
                    "Envelope result is null"));
            }

            if (response.ErrorList is not null || response.IsError)
            {
                return Result.Failure<TResponse, Error>(Error.Failure("envelope.error", "API returned errors"));
            }

            return Result.Success<TResponse, Error>(response.Result);
        }
        catch(Exception ex)
        {
            return Result.Failure<TResponse, Error>(
                Error.Failure("unexpected", $"Unexpected error: {ex.Message}"));
        }
    }

    public static async Task<UnitResult<Error>> HandleResponseAsync(
        this HttpResponseMessage httpReponseMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!httpReponseMessage.IsSuccessStatusCode)
            {
                return UnitResult.Failure(Error.Failure("deserialization.null",
                    "Response deserialization returned null"));
            }

            var response = await httpReponseMessage.Content.ReadFromJsonAsync<Envelope>(cancellationToken);

            if (response is null)
            {
                return UnitResult.Failure(Error.Failure("code", "message"));
            }

            if (response.ErrorList is not null || response.IsError is true)
            {
                return UnitResult.Failure(Error.Failure("envelope.error", "API returned errors"));
            }

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            return UnitResult.Failure(Error.Failure("unexpected", $"Unexpected error: {ex.Message}"));
        }
    }
}