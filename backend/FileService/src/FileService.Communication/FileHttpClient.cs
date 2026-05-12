using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.IntegrationTests.Features;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.Communication;

internal sealed class FileHttpClient : IFileService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileHttpClient> _logger;

    public FileHttpClient(
        HttpClient httpClient,
        ILogger<FileHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<GetMediaAssetInfoResponse, Error>> GetMediaAssetInfo(
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/files/{mediaAssetId}", cancellationToken);
            return await response.HandleResponseAsync<GetMediaAssetInfoResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred when getting media info asset by id {Id}", mediaAssetId);
            return Error.Failure("internal.error", "Failed to get media asset info");
        }
    }

    public async Task<Result<GetMediaAssetsInfoResponse, Error>> GetMediaAssetsInfo(
        GetMediaAssetsInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/files/batch", request, cancellationToken);
            return await response.HandleResponseAsync<GetMediaAssetsInfoResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred when getting media assets by ids {Ids}", string.Join(", ", request.MediaAssetIds));
            return Error.Failure("internal.error", "Failed to get media assets info");
        }
    }

    public async Task<Result<CheckVideoExistenceResponse, Error>> CheckVideoExistence(
        Guid videoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/files/{videoId}/exists", cancellationToken);
            return await response.HandleResponseAsync<CheckVideoExistenceResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred when checking existence of video by id {Id}", videoId);
            return Error.Failure("internal.error", "Failed to check existence of video");
        }
    }
}