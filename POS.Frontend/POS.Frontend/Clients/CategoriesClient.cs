using System.Net.Http.Json;
using System.Text.Json;
using POS.Frontend.Models;

namespace POS.Frontend.Clients;

public sealed class CategoriesClient
{
    private readonly HttpClient _httpClient;

    public CategoriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result<List<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        SendAsync<List<CategoryModel>>(new HttpRequestMessage(HttpMethod.Get, "api/categories"), cancellationToken);

    public Task<Result<CategoryModel>> GetCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(new HttpRequestMessage(HttpMethod.Get, $"api/categories/{id}"), cancellationToken);

    public Task<Result<CategoryModel>> CreateCategoryAsync(
        CategoryFormModel category,
        CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(
            new HttpRequestMessage(HttpMethod.Post, "api/categories")
            {
                Content = JsonContent.Create(category)
            },
            cancellationToken);

    public Task<Result<CategoryModel>> UpdateCategoryAsync(
        int id,
        CategoryFormModel category,
        CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(
            new HttpRequestMessage(HttpMethod.Put, $"api/categories/{id}")
            {
                Content = JsonContent.Create(category)
            },
            cancellationToken);

    public Task<Result<CategoryModel>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync<CategoryModel>(new HttpRequestMessage(HttpMethod.Delete, $"api/categories/{id}"), cancellationToken);

    private async Task<Result<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (request)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ApiException(
                    "The API took too long to respond. Please try again.",
                    innerException: exception);
            }
            catch (HttpRequestException exception)
            {
                throw new ApiException(
                    "The API is unavailable. Make sure it is running and try again.",
                    innerException: exception);
            }

            using (response)
            {
                Result<T>? result;

                try
                {
                    result = await response.Content.ReadFromJsonAsync<Result<T>>(
                        cancellationToken: cancellationToken);
                }
                catch (JsonException exception)
                {
                    throw new ApiException("The API returned an unreadable response.", response.StatusCode, exception);
                }
                catch (NotSupportedException exception)
                {
                    throw new ApiException("The API returned an unsupported response.", response.StatusCode, exception);
                }

                if (result is null)
                {
                    throw new ApiException("The API returned an empty response.", response.StatusCode);
                }

                return result;
            }
        }
    }
}
